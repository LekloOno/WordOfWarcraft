using Godot;
using WowGd.Src.Input;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class BodyMover : Node, IMode
{
    [Export] private float _acceleration;
    [Export] private float _maxSpeed;
    [Export] private float _friction;

    private Body _body = null!;
    private IWishDir _wishDir = null!;

    public override void _Ready()
    {
        if (!this.TryGetComposed(out Body? body))
            return;

        if (!this.TryGetComponent(out IWishDir? wishDir))
            return;

        ModeRegistry.Register(Key.A, this);

        _body = body;
        _wishDir = wishDir;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float) delta;

        Vector2 wishDir = _wishDir.WishDir();

        float currentSpeed = _body.Velocity.Dot(wishDir);
        float t = Mathf.Clamp(currentSpeed / _maxSpeed, 0f, 1f);
        float accelThisFrame = _acceleration * (1f - t) * dt; 
        _body.Velocity += accelThisFrame * wishDir;

        Vector2 drag = -_friction * _body.Velocity;

        if (wishDir != Vector2.Zero)
        {
            float communeDrag = Mathf.Max(0, drag.Dot(-wishDir));
            drag += communeDrag * wishDir;
        }

        _body.Velocity += drag * dt;

        //GD.Print($"speed {_body.Velocity.Length()} | drag {drag} | wishDir {wishDir}");
    }

    public bool Enable() =>
        _wishDir.Enable();

    public bool Disable() =>
        _wishDir.Disable();
}