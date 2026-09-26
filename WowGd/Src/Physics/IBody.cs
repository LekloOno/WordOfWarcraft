using Godot;

namespace WowGd.Src.Physics;

public interface IBody
{
    Vector2 GlobalPosition        { get; }
    Vector2 LinearVelocity  { get; }
    
    void ApplyForce(Vector2 force);
    void ApplyImpulse(Vector2 force);
    void SetVelocity(Vector2 vel);
    Rid GetRid();
    World2D GetWorld2D();
}