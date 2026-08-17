using Godot;
using WowGd.Src.Physics.Movement.Data;

namespace WowGd.Src.Physics.Movement.BodyPhx;

public interface IBodyMovement
{
    public Vector2 ComputeVelocity(Vector2 position, Vector2 velocity, TargetMove move, double delta);
}