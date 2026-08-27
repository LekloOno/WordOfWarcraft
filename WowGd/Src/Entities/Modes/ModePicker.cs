using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities.Modes;

[GlobalClass]
public partial class ModePicker : Node
{
    private ModesManager _manager = null!;

    public override void _Ready()
    {
        this.TryGetComposed(out _manager!);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key && IsSwitch(key))
            _manager.SwitchNext();
        else if (ModeRegistry.TryGetMode(@event, out IMode? mode))
            _manager.Activate(mode);
    }

    private static bool IsSwitch(InputEventKey key) => 
        key.IsPressed() && !key.IsEcho() && key.Keycode == Key.Tab;
}