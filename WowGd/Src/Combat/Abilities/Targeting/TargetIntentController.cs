using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting;

public sealed class TargetIntentController : IDisposable
{
    private readonly IEntity _caster;
    private readonly TargetIntentAcquirer _acquirer;
    private readonly IEnumerable<ITargetRule> _targetRules;
    private readonly CancellationTokenSource _lifetimeCts;

    private IEntity? _trackedDirectTarget;
    private CancellationTokenSource? _reacquireCts;
    private TaskCompletionSource<TargetResult>? _validTargetTcs;

    public TargetIntent Current { get; private set; }
    public bool IsValid { get; private set; }
    public event Action<TargetIntent>? Updated;

    public TargetFailure? LastFailure { get; private set; }
    public event Action<TargetFailure>? Failed;

    public TargetIntentController(
        IEntity caster,
        TargetIntentAcquirer acquirer,
        IEnumerable<ITargetRule> targetRules,
        TargetIntent initial,
        CancellationToken lifetimeToken)
    {
        _caster = caster;
        _acquirer = acquirer;
        _targetRules = targetRules;
        _lifetimeCts = CancellationTokenSource.CreateLinkedTokenSource(lifetimeToken);

        Current = initial;
        IsValid = false;
        TrackDirectTarget(initial);
    }

    // Call on target death, failed loop rules, or explicit player retarget.
    // Repeated calls supersede any in-flight reacquisition. Never touches actuation.
    public void RequestRefresh()
    {
        IsValid = false;
        _reacquireCts?.Cancel();
        _reacquireCts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token);

        _validTargetTcs = CreateValidTargetTcs();

        _ = ReacquireAsync(_reacquireCts.Token);
    }

    public Task<TargetResult> WaitForValidTargetAsync(
        CancellationToken ct = default)
    {
        if (IsValid)
        {
            if (_targetRules.CheckAll(_caster, Current))
                return Task.FromResult(TargetResult.Ok(Current));
            
            RequestRefresh();
        }

        _validTargetTcs ??= CreateValidTargetTcs();
        return _validTargetTcs.Task.WaitAsync(ct);
    }

    private TaskCompletionSource<TargetResult> CreateValidTargetTcs()
    {
        return new TaskCompletionSource<TargetResult>(
            TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private async Task ReacquireAsync(CancellationToken ct)
    {
        try
        {
            TargetResult result;
            TargetIntent intent;

            while (true)
            {
                result = await _caster.TargetIntentDriver.RetrieveTargetIntent(
                    _caster, _acquirer, ct, _targetRules);

                ct.ThrowIfCancellationRequested();

                if (!result.TryGet(out intent))
                {
                    Fail(result);
                    return;
                }

                if (_targetRules.CheckAll(_caster, intent))
                    break;

                // Only Direct is interactive, so only it can safely re-prompt.
                if (_acquirer != TargetIntentAcquirer.Direct)
                {
                    Fail(TargetResult.Fail(TargetFailure.RuleViolation));
                    return;
                }
            }

            Current = intent;
            TrackDirectTarget(intent);
            LastFailure = null;
            IsValid = true;
            _validTargetTcs?.TrySetResult(TargetResult.Ok(intent));

            Updated?.Invoke(intent);
        }
        catch (OperationCanceledException) { }
    }

    private void Fail(TargetResult failure)
    {
        IsValid = false;
        LastFailure = failure.Failure;
        _validTargetTcs?.TrySetResult(failure);
        Failed?.Invoke(failure.Failure!.Value);
    }

    private void TrackDirectTarget(TargetIntent intent)
    {
        if (_trackedDirectTarget != null)
            _trackedDirectTarget.Health.Died -= OnDirectTargetDied;

        _trackedDirectTarget = intent.TryGetEntity(out var entity) ? entity : null;

        if (_trackedDirectTarget != null)
            _trackedDirectTarget.Health.Died += OnDirectTargetDied;
    }

    private void OnDirectTargetDied() => RequestRefresh();

    public void Dispose()
    {
        if (_trackedDirectTarget != null)
            _trackedDirectTarget.Health.Died -= OnDirectTargetDied;
        _reacquireCts?.Cancel();
        _lifetimeCts.Cancel();

        _validTargetTcs?.TrySetCanceled(_lifetimeCts.Token);
    }
}