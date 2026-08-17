using Godot;

namespace WowGd.Src.Input.Targeting.Free.Confirm;

[GlobalClass]
public partial class CursorInputConfirm : CursorConfirm
{
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent)
            return;

        if (keyEvent.Keycode == Key.Space)
            _cursor.Confirm();
    }
}