using Godot;

namespace WowGd.Src.Input.Targeting.Free.Confirm;

[GlobalClass]
public partial class CursorAutoConfirm : CursorConfirm
{
    public CursorAutoConfirm() {}
    public CursorAutoConfirm(Cursor cursor) : base(cursor) {}

    public override void _PhysicsProcess(double delta)
    {
        _cursor.Confirm();
    }
}