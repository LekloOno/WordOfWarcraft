using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Confirm;

[GlobalClass]
public partial class SwappableCursorConfirm : Node
{
    public enum CursorConfirmType
    {
        Input,
        Auto,
    }

    private Cursor _cursor = null!;
    private CursorConfirm _confirm = null!;
    private CursorConfirmType _type;

    [Export]
    public CursorConfirmType Type
    {
        get => _type;
        set
        {
            if (value == _type)
                return;

            _type = value;
            _confirm?.QueueFree();

            if (_cursor == null)
                return;

            _confirm = Generate(value);
            AddChild(_confirm);
        }
    }

    private CursorConfirm Generate(CursorConfirmType type)
    {
        return type switch
        {
            CursorConfirmType.Input => new CursorInputConfirm(_cursor),
            CursorConfirmType.Auto => new CursorAutoConfirm(_cursor),
            _ => new CursorAutoConfirm(_cursor),
        };
    }

    public override void _Ready()
    {
        if (!this.TryGetComposed(out Cursor? cursor))
            return;
        
        _cursor = cursor;

        _confirm = Generate(_type);
        AddChild(_confirm);
    }
}