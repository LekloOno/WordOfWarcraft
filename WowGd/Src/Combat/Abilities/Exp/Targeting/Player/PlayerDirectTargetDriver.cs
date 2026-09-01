using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Entities;
using WowGd.Src.Input;
using WowGd.Src.Input.Targeting.Direct;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting.Player;

public partial class PlayerDirectTargetDriver : Node
{
    private TargetIntent? _buffered;
    private TaskCompletionSource<TargetIntent>? _pendingIntent;

    public override void _Ready()
    {
        SetProcessUnhandledKeyInput(false);
    }

    public async Task<TargetIntent> RetrieveTarget(IEntity caster, IEnumerable<ITargetRule>? rules, CancellationToken ct)
    {
        // See if we later put some domain specific cancellation ?
        if (_pendingIntent is not null)
            throw new InvalidOperationException(
                "A target acquisition is already in progress.");

        if (_buffered is TargetIntent intent)
            return intent;

        var pendingIntent = new TaskCompletionSource<TargetIntent>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _pendingIntent = pendingIntent;
        EnableTargeting(caster, rules);

        try
        {
            using var registration = ct.Register(
                () => _pendingIntent.TrySetCanceled(ct));

            return await _pendingIntent.Task;
        }
        finally
        {
            _pendingIntent = null;
            DisableTargeting();
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
        if (_pendingIntent is null)
        return;

        if (TryGetTargetIntent(@event, out TargetIntent intent))
        {
            _pendingIntent.TrySetResult(intent);
            _buffered = intent;
        }
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
}