using Godot;
using WowGd.Src.Input.Targeting.Free;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Generators;

[GlobalClass]
public partial class RelativeKeyGenerator : Node, IVec2InputGenerator
{
    private Vector2 _relativePosition = Vector2.Zero;
    private bool _buffered = false;
    private readonly ProcessToggle _toggle = new();

    public override void _Ready()
    {
        _toggle.Toggled += SetProcessUnhandledKeyInput;
        _toggle.Toggled += ResetBuffer;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey eventKey)
            return;

        if (!@event.IsPressed() || @event.IsEcho())
            return;

        if (!GridKey.TryGetPosition(eventKey.Keycode, out Vector2 pos))
            return;

        _relativePosition = pos;
        _buffered = true;
    }

    public bool Retrieve(out Vector2 vec)
    {
        vec = default;

        if (!_buffered)
            return false;

        _buffered = false;
        vec = _relativePosition;
        return true;
    }

    public bool Enable()    => _toggle.Enable();
    public bool Disable()   => _toggle.Disable();

    private void ResetBuffer(bool _)
        => _buffered = false;
}