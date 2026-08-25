using Godot;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Tools;

namespace WowGd.Src.Dactylo.Input;

[GlobalClass]
public partial class DactyloInput : Node, IDisablable
{
    // temporary
    private Node _worderNode = null!;
    [Export] public Node Worder
    {
        get => _worderNode;
        set
        {
            if (_worderNode == value)
                return;
            
            _worderNode = value;
            _worderNode.TryDeriveComponent(out _worder!);
        }
    }

    public bool Enabled => _enabled;
    private bool _enabled = true;

    private IWorder _worder = null!;

    public bool Disable() =>
        DisableExt.IndempDisable(ref _enabled, () => SetProcessUnhandledKeyInput(false));

    public bool Enable() =>
        DisableExt.IndempEnable(ref _enabled, () => SetProcessUnhandledKeyInput(true));

    public override void _Ready()
    {
        _worderNode.TryDeriveComponent(out _worder!);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key)
            return;

        if (!key.IsPressed() || key.IsEcho())
            return;

        _worder.Process(key);
    }
}
