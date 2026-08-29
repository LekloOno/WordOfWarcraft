using Godot;

namespace WowGd.Src.Physics;

public interface IBody
{
    Vector2 GlobalPosition        { get; }
    Vector2 LinearVelocity  { get; }
    float AngularVelocity   { get; }
    
    void ApplyForce(Vector2 force);
    Rid GetRid();
    World2D GetWorld2D();
}