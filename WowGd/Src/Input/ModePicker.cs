using Godot;

namespace WowGd.Src.Input;

[GlobalClass]
public partial class ModePicker : Node
{   
    private IMode? _current;
    public override void _Input(InputEvent @event)
    {
        if (!ModeRegistry.TryGetMode(@event, out IMode? mode))
            return;

        if (_current == mode)
            return;

        _current?.Disable();
        _current = mode;
        _current.Enable();
    }
}