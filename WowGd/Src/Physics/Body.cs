using System;
using Godot;
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
    // For interpollation
    public Vector2 PrevPosition { get; private set; } = Vector2.Zero;
    public Vector2 Position     { get; private set; } = Vector2.Zero;

    private Vector2 _velocity = Vector2.Zero;

    // avoids division by ~0 when computing r̂
    private const float ArrivalEpsilon = 0.02f;
    // below this speed, "current velocity direction" is meaningless 
    private const float StillEpsilon = 0.02f;
    
    private TargetMove _move;
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

    // This thing is WIP.
    // The current behavior isn't ideal yet, need more accurate kinematic to avoid perpetual zoomies correction.
    public override void _PhysicsProcess(double delta)
    {
        PrevPosition = Position;

        float dt = (float)delta;

        Vector2 toTarget = _move.Target - Position;
        float   d = toTarget.Length();

        bool hadReachedTarget = _reachedTarget;
        _reachedTarget = d < ArrivalEpsilon;

        if (_reachedTarget)
        {
            if (!hadReachedTarget)
                ReachedTarget?.Invoke();

            if (_velocity.LengthSquared() < StillEpsilon * StillEpsilon)
            {
                _velocity = Vector2.Zero;
                return;
            }
        }
        
        Vector2 rHat = d > ArrivalEpsilon ? toTarget / d : Vector2.Zero;

        float   vR = _velocity.Dot(rHat);
        //Vector2 vT = _velocity - vR * rHat;

        float radialClosing = Mathf.Max(vR, 0f);
        float dStop = _move.Deceleration > 0f
            ? (radialClosing * radialClosing) / (2f * _move.Deceleration)
            : 0f;

        float vRDesired;
        if (d > dStop)
            vRDesired = _move.MaxSpeed;
        else
            vRDesired = Mathf.Sqrt(Mathf.Max(0f, 2f * _move.Deceleration * d));

        Vector2 vDesired    = vRDesired * rHat;
        Vector2 aRaw        = (vDesired - _velocity) / dt;

        float bound;
        if (_velocity.LengthSquared() < StillEpsilon * StillEpsilon)
            bound = _move.Acceleration;
        else
            bound = aRaw.Dot(_velocity) < 0f ? _move.Deceleration : _move.Acceleration;

        Vector2 aFinal = aRaw;
        float aRawLenSq = aRaw.LengthSquared();
        if (bound > 0f && aRawLenSq > bound * bound)
            aFinal = aRaw / Mathf.Sqrt(aRawLenSq) * bound;

        _velocity += aFinal * dt;

        if (_velocity.LengthSquared() > _move.MaxSpeed * _move.MaxSpeed && _move.MaxSpeed > 0f)
            _velocity = _velocity.Normalized() * _move.MaxSpeed;

        Position += _velocity * dt;
    }
}