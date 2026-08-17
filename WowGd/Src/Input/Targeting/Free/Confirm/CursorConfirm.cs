using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Confirm;

public abstract partial class CursorConfirm : Node
{
    public CursorConfirm() {}
    public CursorConfirm(Cursor cursor)
    {
        _cursor = cursor;
    }

    protected Cursor _cursor = null!;

    public override sealed void _Ready()
    {
        if (_cursor != null)
            return;

        if (!this.TryGetComposed(out Cursor? cursor))
            return;

        _cursor = cursor;
    }
}