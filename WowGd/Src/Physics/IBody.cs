using Godot;

namespace WowGd.Src.Physics;

public interface IBody
{
    Vector2 GlobalPosition  { get; }
    
    Vector2 RawVelocity     { get; }
    Vector2 Inertia         { get; }
    
    /// <summary>
    /// Raw forces is a buffer that gets reseted at each new tick.
    /// It is applied with no friction, no inertia, just raw force.
    /// </summary>
    /// <param name="raw"></param>
    void AddRawForce(Vector2 raw);
    /// <summary>
    /// Accelerate increments the inertia of the body.
    /// It is a separate buffer from raw forces, is not rested per tick,
    /// and can evolve from the physic simulation itself, independantly of the entity's mover, from collisions for example.
    /// </summary>
    /// <param name="accel"></param>
    void Accelerate(Vector2 accel);

    Rid GetRid();
    World2D GetWorld2D();
}