using Godot;

namespace WowGd.Src.Input;

[GlobalClass]
public partial class ModePicker : Node
{   
    private IMode? _current;

    public override void _Ready()
    {
        _current?.Activate();
    }

    public override void _Input(InputEvent @event)
    {
        if (!ModeRegistry.TryGetMode(@event, out IMode? mode))
            return;

        Select(mode);
    }

    public bool Select(IMode mode)
    {
        if (_current == mode)
            return true;

        _current?.Deactivate();

        if (!mode.Activate())
            return false;

        _current = mode;
        return true;
    }

    public void Preselect(IMode mode)
    {
        _current = mode;
    }
}