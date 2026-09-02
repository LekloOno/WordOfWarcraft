using System.Threading;
using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Player;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Direct;

[GlobalClass]
public partial class DirectInputBufferTrigger : Node
{
    [Export] private PlayerTargetIntentDriver _targetIntentDrive = null!;
    private IEntity _entity = null!;

    public override void _Ready()
    {
        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("focus_target"))
        {
            if (_targetIntentDrive.DirectDriver.BufferAcquisition)
                return;

            _targetIntentDrive.DirectDriver.StartBufferTarget(_entity, null);
            return;
        }
        
        if(@event.IsActionReleased("focus_target"))
        {
            if (!_targetIntentDrive.DirectDriver.BufferAcquisition)
                return;

            _targetIntentDrive.DirectDriver.StopBufferTarget();
        }
    }
}