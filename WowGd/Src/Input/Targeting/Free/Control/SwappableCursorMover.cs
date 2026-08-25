using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Control;

[GlobalClass]
public partial class SwappableCursorMover : Node
{
    public enum CursorMoverType
    {
        RelativeKey,
        StandardKey,
    }

    private Cursor _cursor = null!;
    private CursorMoverBase _mover = null!;
    private CursorMoverType _type;

    [Export]
    public CursorMoverType Type
    {
        get => _type;
        set
        {
            if (value == _type)
                return;

            _type = value;

            Replace(_type);
        }
    }

    private CursorMoverBase Generate(CursorMoverType type)
    {
        return type switch
        {
            CursorMoverType.RelativeKey => new RelativeKeyCursorMover(_cursor),
            CursorMoverType.StandardKey => new StandardKeyCursorMover(_cursor),
            _ => new StandardKeyCursorMover(_cursor),
        };
    }

    private void Replace(CursorMoverType type)
    {
        CursorMoverBase prev = _mover;

        _mover?.QueueFree();

        if (_cursor == null)
            return;

        _mover = Generate(type);
        AddChild(_mover);
        
        if (prev is null)
            return;

        //if (prev.Enabled)
        //    _mover.Enable();
        //else
        //    _mover.Disable();
    }

    public override void _Ready()
    {
        if (!this.TryGetComposed(out Cursor? cursor))
            return;
        
        _cursor = cursor;

        _mover = Generate(_type);
        AddChild(_mover);
    }
}