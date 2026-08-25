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
public partial class Body : RigidBody2D, IBody
{
    void IBody.ApplyForce(Vector2 force) => ApplyForce(force);
}
