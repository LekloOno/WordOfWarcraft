using Godot;

namespace WowGd.Src.Debug;

[GlobalClass]
public partial class WrittenInputIntercept : Node
{
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey k)
            return;

        long unicode = k.Unicode;
        if (unicode != 0)
            GD.Print(((char)unicode).ToString());       
    }
}