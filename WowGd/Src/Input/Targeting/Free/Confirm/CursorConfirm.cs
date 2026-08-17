using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Confirm;

public abstract partial class CursorConfirm : Node
{
    protected Cursor _cursor = null!;

    public override sealed void _Ready()
    {
        if (!this.TryGetComposed(out Cursor? cursor))
            return;

        _cursor = cursor;
    }
}