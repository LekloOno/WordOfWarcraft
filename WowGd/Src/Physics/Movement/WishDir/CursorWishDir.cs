using Godot;
using WowGd.Src.Input.Targeting.Free;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.WishDir;

[GlobalClass]
public partial class CursorWishDir : Node, IWishDir
{
    private const float TargetReachedEpsilon = 0.02f;

    [Export] private Body _body = null!;
    private Cursor _cursor = null!;
    private readonly ProcessToggle _toggle = new();

    public override void _Ready()
    {
        if (this.TryGetComponent(out Cursor? cursor))
            _cursor = cursor;
    }

    public bool Disable()
    {
        _cursor.Disable();
        return _toggle.Disable();
    }

    public bool Enable()
    {
        _cursor.Enable();
        return _toggle.Enable();
    }

    public Vector2 WishDir()
    {
        Vector2 dir = _cursor.Target - _body.Position;
        
        if (dir.LengthSquared() < TargetReachedEpsilon * TargetReachedEpsilon)
            return Vector2.Zero;

        return dir.Normalized();
    }
}