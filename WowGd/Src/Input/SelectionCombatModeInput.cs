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

        if (@event.TryGetIndex(out int index))
            _mode.Select(index);
    }
}