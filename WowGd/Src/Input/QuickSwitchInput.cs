using Godot;
using WowGd.Src.Input.Hands;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Input;

[GlobalClass]
public partial class QuickSwitchInput : Node
{
    [Export] private SelectionCombatModeInput _selectionInput = null!;
    [Export] private MoveModeInput _moveModeInput = null!;

    private bool _active;
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!@event.IsAction("quick_switch"))
            return;

        if (@event.IsEcho())
            return;

        if (@event.IsPressed())
        {
            if (_active)
                return;

            _active = true;
            HandsInputManager.TryPushSecondHandMode(_selectionInput);
            HandsInputManager.TryPushFirstHandMode(_moveModeInput);
        }
        else
        {
            if (!_active)
                return;

            _active = false;
            HandsInputManager.TryRemoveSecondHandMode(_selectionInput);
            HandsInputManager.TryRemoveFirstHandMode(_moveModeInput);
        }
    }
}