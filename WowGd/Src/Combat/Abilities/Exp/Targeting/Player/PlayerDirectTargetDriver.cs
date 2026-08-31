using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting.Player;

public partial class PlayerDirectTargetDriver : Node
{
    private TargetIntent? _buffered;
    private TaskCompletionSource<TargetIntent>? _pendingIntent;

    public override void _Ready()
    {
        SetProcessUnhandledKeyInput(false);
    }

    public async Task<TargetIntent> RetrieveTarget(CancellationToken ct)
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
        EnableTargeting();

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
    }

    private void EnableTargeting()
    {
        SetProcessUnhandledKeyInput(true);
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

        if (@event is not InputEventKey key || @event.IsReleased())
            return false;

        // .. The rest of the logic to develop ...

        return false;
    }
}