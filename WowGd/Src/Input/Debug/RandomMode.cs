using Godot;

namespace WowGd.Src.Input.Debug;

[GlobalClass]
public partial class RandomMode : Node, IMode
{
    [Export] private TextEdit _textEdit = null!;

    public override void _Ready()
    {
        ModeRegistry.Register(Key.K, this);
    }

    public bool Disable()
    {
        _textEdit.ReleaseFocus();
        return true;
    }
    public bool Enable()
    {
        _textEdit.GrabFocus();
        return true;
    }
}