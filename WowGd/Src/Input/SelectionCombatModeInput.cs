using Godot;
using WowGd.Src.Combat.Modes;

namespace WowGd.Src.Input;

[GlobalClass]
public partial class SelectionCombatModeInput : Node
{
    [Export] private SelectionCombatMode _mode = null!;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (_mode.HasActive())
        {
            if (@event.IsActionPressed("ability_cancel"))
                _mode.Unselect(0);  // ad-hoc, this is wip, this would suppose no one else is interracting with the selection
            return;
        }

        if (@event.IsActionPressed("ability_select_0"))
            _mode.Select(0);
        else if (@event.IsActionPressed("ability_select_1"))
            _mode.Select(1);
        else if (@event.IsActionPressed("ability_select_2"))
            _mode.Select(2);
        else if (@event.IsActionPressed("ability_select_3"))
            _mode.Select(3);
        else if (@event.IsActionPressed("ability_select_4"))
            _mode.Select(4);
        else if (@event.IsActionPressed("ability_select_5"))
            _mode.Select(5);
        else if (@event.IsActionPressed("ability_select_6"))
            _mode.Select(6);
        else if (@event.IsActionPressed("ability_select_7"))
            _mode.Select(7);
        else if (@event.IsActionPressed("ability_select_8"))
            _mode.Select(8);
        else if (@event.IsActionPressed("ability_select_9"))
            _mode.Select(9);
    }
}