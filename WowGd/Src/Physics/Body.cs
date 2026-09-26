using Godot;

namespace WowGd.Src.Physics;

/// <summary>
/// Holds and computes the physics of a targeteable body.
///
/// That is a body that can be assigned to reach a given destination.
///
/// It's not deriving Node2D nor Node3D, as it is a pure logic Node.
/// Representation should be handled separetely, as we're still in early prototyping phase,
/// and don't have a fixed view on what we want the game to look like.
/// Fully separating the logic can only be a good thing anyways.
/// </summary>
[GlobalClass]
public partial class Body : CharacterBody2D, IBody
{
    public Vector2 LinearVelocity => Velocity;

    private Vector2 _frameForces;

    void IBody.ApplyForce(Vector2 force)
    {
        _frameForces += force;
    }

    void IBody.ApplyImpulse(Vector2 impulse)
    {
        Velocity += impulse;
    }

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Floating;
    }

    public override void _PhysicsProcess(double delta)
    {
        //Velocity = _frameForces;
        //_frameForces = Vector2.Zero;
        MoveAndSlide();
    }
}
