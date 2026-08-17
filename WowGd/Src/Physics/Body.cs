using System;
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
public partial class Body : Node
{
    // For interpollation
    public Vector2 PrevPosition { get; private set; } = Vector2.Zero;
    public Vector2 Position     { get; private set; } = Vector2.Zero;

    public Vector2 Velocity = Vector2.Zero;

    public const float ArrivalEpsilon = 0.05f;
    
    private bool _reachedTarget = false;
    /// <summary>
    /// Emitted once the current target has been reached.
    /// </summary>
    public event Action? ReachedTarget;

    public Vector2 GetInterpollatedPosition()
    {
        var alpha = Engine.GetPhysicsInterpolationFraction();
        return PrevPosition.Lerp(Position, (float) alpha);
    }

    public override void _PhysicsProcess(double delta)
    {
        PrevPosition = Position;
        Position += Velocity * (float) delta;
    }
}