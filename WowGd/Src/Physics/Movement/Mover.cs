using Godot;
using WowGd.Src.Physics.Movement.Data;

namespace WowGd.Src.Physics.Movement;

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
            GD.PushError($"[{nameof(Mover)}] requires a [{nameof(Body)}] parent.");
            return;
        }

        _body = body;
        _body.ReachedTarget += OnReachedTarget;
    }

    public void RequestMoveTo(Vector2 position)
    {
        _move.Target = position;

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
        GD.Print("unstun");
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