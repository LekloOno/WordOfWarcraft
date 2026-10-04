using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Entities;
using WowGd.Src.Input;
using WowGd.Src.Input.Hands;
using WowGd.Src.Input.Targeting.Direct;

namespace WowGd.Src.Combat.Abilities.Targeting.Player;

public partial class PlayerDirectTargetDriver : Node, ISecondHandInputMode, IListenableHandInputMode
{
    private TargetIntent? _buffered;
    private TaskCompletionSource<TargetResult>? _pendingIntent;

    private bool _defaultToNearest = true;

    public override void _Ready()
    {
        SetProcessUnhandledKeyInput(false);
    }

    private bool _processing = false;

    public event Action? InputStarted;
    public event Action? InputStopped;
    public event Action? InputPushed;
    public event Action? InputRemoved;


    public async Task<TargetResult> RetrieveTarget(IEntity caster, IEnumerable<ITargetRule>? rules, CancellationToken ct, bool useBuffer = true)
    {
        // See if we later put some domain specific cancellation ?
        if (_pendingIntent is not null)
            throw new InvalidOperationException(
                "A target acquisition is already in progress.");

        if (useBuffer && _buffered is TargetIntent intent &&
            (rules?.CheckAll(caster, intent) ?? true))
            return TargetResult.Ok(intent);

        if (_defaultToNearest && DirectTargetEntitiesManager.TryRetrieveClosestEntity(caster, out IEntity? entity, rules))
        {
            intent = new(entity);
            _buffered = intent;
            return TargetResult.Ok(intent);
        }

        var pendingIntent = new TaskCompletionSource<TargetResult>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _pendingIntent = pendingIntent;
        
        _processing = true;
        EnableTargeting(caster, rules);
        if (HandsInputManager.TryPushSecondHandMode(this))
            InputPushed?.Invoke();

        try
        {
            using var registration = ct.Register(
                () => pendingIntent.TrySetCanceled(ct));

            return await pendingIntent.Task;
        }
        finally
        {
            _processing = false;
            _pendingIntent = null;
            DisableTargeting();
            if (HandsInputManager.TryRemoveSecondHandMode(this))
                InputRemoved?.Invoke();
        }
    }

    private void DisableTargeting()
    {
        SetProcessUnhandledKeyInput(false);
        DirectTargetEntitiesManager.Disable();
    }

    private void EnableTargeting(IEntity caster, IEnumerable<ITargetRule>? rules)
    {
        SetProcessUnhandledKeyInput(true);
        DirectTargetEntitiesManager.Enable(caster, rules);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!TryGetTargetIntent(@event, out TargetIntent intent))
            return;

        if (_pendingIntent is null)
            SetBuffer(intent);
        else if (_pendingIntent.TrySetResult(TargetResult.Ok(intent)))
            SetBuffer(intent);
    }

    private void SetBuffer(TargetIntent intent)
    {
        if (_buffered is TargetIntent targetIntent)
            targetIntent.Entity!.Health.Died -= OnBufferedDied;

        _buffered = intent;
        intent.Entity!.Health.Died += OnBufferedDied;
    }

    private static bool TryGetTargetIntent(InputEvent @event, out TargetIntent intent)
    {
        intent = default;

        // Ability for now, just for test sake, will use proper map later
        if (!@event.TryGetAbilityIndex(out int index))
            return false;

        if (!DirectTargetEntitiesManager.TryRetrieveEntity(index, out IEntity? entity))
            return false;

        intent = new(entity);
        return true;
    }

    private void OnBufferedDied()
    {
        _buffered = null;
    }

    public bool CanStart() => true;
    public void Start()
    {
        if (_processing || BufferAcquisition)
        {
            SetProcessUnhandledKeyInput(true);
            InputStarted?.Invoke();
        }
    }
    public bool CanStop() => true;
    public void Stop()
    {
        SetProcessUnhandledKeyInput(false);
        InputStopped?.Invoke();
    }

    public bool BufferAcquisition { get; private set; } = false;
    public bool StartBufferTarget(IEntity caster, IEnumerable<ITargetRule>? rules)
    {
        if (_processing || BufferAcquisition)
            return false;

        BufferAcquisition = true;
        EnableTargeting(caster, rules);
        bool pushed = HandsInputManager.TryPushSecondHandMode(this);
        if (pushed)
            InputPushed?.Invoke();

        return pushed;
    }

    public bool StopBufferTarget()
    {
        if (_processing || !BufferAcquisition)
            return false;
            
        BufferAcquisition = false;
        DisableTargeting();
        bool removed = HandsInputManager.TryRemoveSecondHandMode(this);
        if (removed)
            InputRemoved?.Invoke();

        return removed;
    }
}