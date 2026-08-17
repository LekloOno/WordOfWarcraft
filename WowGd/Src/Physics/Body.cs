using System;
using Godot;
using WowGd.Src.Physics.Movement.BodyPhx;
using WowGd.Src.Physics.Movement.Data;

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
    private IBodyMovement _physics = null!;

    // For interpollation
    public Vector2 PrevPosition { get; private set; } = Vector2.Zero;
    public Vector2 Position     { get; private set; } = Vector2.Zero;

    private Vector2 _velocity = Vector2.Zero;

    public const float ArrivalEpsilon = 0.05f;
    
    private TargetMove _move;
    private bool _reachedTarget = false;
    /// <summary>
    /// Emitted once the current target has been reached.
    /// </summary>
    public event Action? ReachedTarget;

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is not IBodyMovement mvt)
                continue;

            _physics = mvt;
            return;
        }

        GD.PushError($"[{nameof(Body)}] requires a [{nameof(IBodyMovement)}] children.");
    }

    public Vector2 GetInterpollatedPosition()
    {
        var alpha = Engine.GetPhysicsInterpolationFraction();
        return PrevPosition.Lerp(Position, (float) alpha);
    }

    // A dedicated function is probably more epxlicite than a setter.
    public void SetMove(TargetMove move)
    {
        _move = move;

        Vector2 toTarget = _move.Target - Position;
        float d = toTarget.Length();
        
        bool hadReachedTarget = _reachedTarget;
        _reachedTarget = d < ArrivalEpsilon;

        if (_reachedTarget && !hadReachedTarget)
            ReachedTarget?.Invoke();
    }

    public override void _PhysicsProcess(double delta)
    {
        PrevPosition = Position;

        _velocity = _physics.ComputeVelocity(Position, _velocity, _move, delta);
        Position += _velocity * (float) delta;

        Vector2 toTarget = _move.Target - Position;
        float   d = toTarget.Length();

        bool hadReachedTarget = _reachedTarget;
        _reachedTarget = d < ArrivalEpsilon;

        if (!hadReachedTarget && _reachedTarget)
            ReachedTarget?.Invoke();
    }
}