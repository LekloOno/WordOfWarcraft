
using Godot;
using WowGd.Src.Physics.Movement.Data;

namespace WowGd.Src.Physics.Movement.BodyPhx;

[GlobalClass]
public partial class BodyTranslator : Node, IBodyMovement
{
    public Vector2 ComputeVelocity(Vector2 position, Vector2 velocity, TargetMove move, double delta)
    {
        float dt = (float)delta;

        Vector2 toTarget = move.Target - position;
        float   d = toTarget.Length();

        if (d < Body.ArrivalEpsilon)
            return Vector2.Zero;

        GD.Print($"{toTarget.Length()/dt} - {move.MaxSpeed}");
        float speed = Mathf.Min(toTarget.Length()/dt, move.MaxSpeed);
        return toTarget.Normalized() * speed;
    }
}