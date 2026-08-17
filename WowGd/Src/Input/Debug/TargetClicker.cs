using Godot;
using WowGd.Src.Tools;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Physics;

namespace WowGd.Src.Input.Debug;

/// <summary>
/// Just a little debug tool to quickly test the movement physics.
/// 
/// The real game will not be based on clicks, it was just faster to implement and test.
/// </summary>
[GlobalClass]
public partial class TargetClicker : Node, IWishDir
{
    private const float TargetReachedEpsilon = 0.02f;

    [Export] private Body _body = null!;
    private Vector2 _target;

    public bool Disable()
    {
        SetProcessUnhandledInput(false);
        return true;
    }

    public bool Enable()
    {
        SetProcessUnhandledInput(true);
        return true;
    }

    public Vector2 WishDir()
    {
        Vector2 dir = _target - _body.Position;
        
        if (dir.LengthSquared() < TargetReachedEpsilon * TargetReachedEpsilon)
            return Vector2.Zero;

        return dir.Normalized();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
        {
            if (MousePhysics.TryGetMouseWorldPosition(out Vector3 position, GetViewport()))
                _target = position.ToVector2();
        }
    }
}