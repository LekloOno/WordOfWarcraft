using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Render.Animation.Smoothing;

public partial class PositionVelDamper(PositionVelDamperData data)
{
    public PositionVelDamperData Data = data;
    private Vector2 _velocity;
    public Vector2 Position { get; private set; } = Vector2.Zero;

    public void Process(Vector2 target, float delta)
    {
        Vector2 desiredVelocity = (target - Position) * Data.Speed;
        _velocity = _velocity.ProcessLerp(desiredVelocity, Data.VelDamping, delta);

        Position += _velocity * delta;
    }

    public void ResetTo(Vector2 pos) =>
        Position = pos;
}