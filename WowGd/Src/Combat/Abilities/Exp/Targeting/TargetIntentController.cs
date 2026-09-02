using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting;

public sealed class TargetIntentController : IDisposable
{
    private readonly IEntity _caster;
    private readonly TargetIntentAcquirer _acquirer;
    private readonly IEnumerable<ITargetRule> _targetRules;
    private readonly CancellationTokenSource _lifetimeCts;

    private IEntity? _trackedDirectTarget;
    private CancellationTokenSource? _reacquireCts;
    private TaskCompletionSource<TargetIntent>? _validTargetTcs;

    public TargetIntent Current { get; private set; }
    public bool IsValid { get; private set; }
    public event Action<TargetIntent>? Updated;

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

    public Task<TargetIntent> WaitForValidTargetAsync(
        CancellationToken ct = default)
    {
        if (IsValid && _targetRules.CheckAll(_caster, Current))
            return Task.FromResult(Current);

        _validTargetTcs ??= CreateValidTargetTcs();

        return _validTargetTcs.Task.WaitAsync(ct);
    }

    private TaskCompletionSource<TargetIntent> CreateValidTargetTcs()
    {
        return new TaskCompletionSource<TargetIntent>(
            TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private async Task ReacquireAsync(CancellationToken ct)
    {
        try
        {
            TargetIntent intent;
            do
            {
                intent = await _caster.TargetIntentDriver.RetrieveTargetIntent(
                    _caster, _acquirer, ct, _targetRules);
            }
            while (!_targetRules.CheckAll(_caster, intent));

            Current = intent;
            TrackDirectTarget(intent);
            _validTargetTcs?.TrySetResult(intent);
            IsValid = true;

            Updated?.Invoke(intent);
        }
        catch (OperationCanceledException) { }
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