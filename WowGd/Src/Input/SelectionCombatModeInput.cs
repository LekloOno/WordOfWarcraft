using Godot;
using WowGd.Src.Combat.Modes;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Input;

[GlobalClass]
public partial class SelectionCombatModeInput : Node, ISecondHandInputMode
{
    [Export] private SelectionCombatMode _mode = null!;

    private bool _active;

    public override void _Ready()
    {
        HandsInputManager.TryPushSecondHandMode(this);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (_mode.HasActive())
        {
            if (@event.IsActionPressed("ability_cancel"))
                _mode.Unselect(0);  // ad-hoc, this is wip, this would suppose no one else is interracting with the selection
            return;
        }

        if (!_active)
            return;

        if (@event.TryGetAbilityIndex(out int index))
            _mode.Select(index);
    }

    public bool CanStart() => true;
    public bool CanStop() => true;

    public void Start() =>
        _active = true;

    public void Stop() =>
        _active = false;
}