using Godot;

namespace WowGd.Src.Physics;

public interface IBody
{
    Vector2 Position        { get; }
    Vector2 LinearVelocity  { get; }
    float AngularVelocity   { get; }
    
    void ApplyForce(Vector2 force);
}