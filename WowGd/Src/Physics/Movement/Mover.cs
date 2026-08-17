using Godot;
using WowGd.Src.Physics.Movement.Data;

namespace WowGd.Src.Physics.Movement;

/// <summary>
/// The mover has two modes - run and dash.
/// 
/// Movement that are short enough are transformed into a fast and snappy dash.
/// However, the player can't move for a brief moment after a dash. It is used to dodge or quickly reposition, not to rotate.
/// 
/// Below the _dashData.MaxDistance, the player moves normally.
/// 
/// This script also includes a little buffer mechanism.
/// When a movement action is registered as the mover is still in a stun status, it will be buffered then sent and as soon as the stun ends.
/// </summary>
[GlobalClass]
public partial class Mover : Node
{
    [Export] private DashData   _dashData   = null!;
    [Export] private RunData    _runData    = null!;

    private Body _body = null!;

    private bool        _stunned  = false;
    private TargetMove  _move     = new();
    private bool        _buffered = false;

    public override void _Ready()
    {
        SetPhysicsProcess(false);

        if (GetParent() is not Body body)
        {
            GD.PushError($"[{nameof(Mover)}] requires a [{nameof(BodyPhx)}] parent.");
            return;
        }

        _body = body;
        _body.ReachedTarget += OnReachedTarget;
    }

    public void RequestMoveTo(Vector2 position)
    {
        _move.Target = position;

        Vector2 toTarget = _move.Target - _body.Position;
        float d = toTarget.Length();
        bool arrived = d < Body.ArrivalEpsilon;

        if (arrived)
            return;
        if (_stunned)
            _buffered = true;
        else
            Move();
    }

    private void Move()
    {
        float distance = (_move.Target - _body.Position).LengthSquared();

        // Cheaper to ^2 than to ^0.5. I probably have mental illness
        if (distance < _dashData.MaxDistance * _dashData.MaxDistance)
        {
            _move.MaxSpeed       = _dashData.Speed;
            _move.Acceleration   = _dashData.Acceleration;
            _move.Deceleration   = _dashData.Deceleration;

            _stunned = true;
        }
        else
        {
            _move.MaxSpeed       = _runData.Speed;
            _move.Acceleration   = _runData.Acceleration;
            _move.Deceleration   = _runData.Deceleration;
        }

        _body.SetMove(_move);
    }

    private void OnReachedTarget()
    {
        if (_stunned && !IsPhysicsProcessing())
            StartUnstunTimer();
    }


    // ============
    // STUN RELATED
    // ============
    private double _acc = 0f;
    public override void _PhysicsProcess(double delta)
    {
        _acc += delta;
        
        if (_acc >= _dashData.StunDuration)
            Unstun();   
    }

    private void Unstun()
    {
        SetPhysicsProcess(false);
        _stunned = false;

        if (!_buffered)
            return;

        Move();
        _buffered = false;
    }

    private void StartUnstunTimer()
    {
        _acc = 0f;
        SetPhysicsProcess(true);
    }
}