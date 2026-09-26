using Godot;
using WowGd.Src.Physics.Movement.Channels.Internal;
using WowGd.Src.Tools;

namespace WowGd.Src.Render.Camera;

[GlobalClass]
public partial class BodyWishDirPredictor : Node2D
{
    [Export] private BodyMover _bodyMover = null!;
    /// <summary>
    /// Delay before camera starts diving when wish dir is not Vector2.Zero
    /// </summary>
    [Export] private float _inDelay = 0.5f;
    /// <summary>
    /// Delay before camera resets when wish dir is Vector2.Zero
    /// </summary>
    [Export] private float _outDelay = 0.2f;
    [Export] private float _maxWishDirVel = 5f;
    [Export] private float _distance = 2f;
    [Export] private float _velDamping = 2f;
    private Vector2 _bufferedWishDir;
    private Vector2 _smoothedWishDir;
    private bool _active = false;
    private float _acc = 0f;

    private Vector2 _velocity;

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        _acc += dt;

        _bufferedWishDir = _active
            ? GetActiveTargetWishDir(dt)
            : GetUnactiveTargetWishDir(dt);

        Vector2 desiredVelocity = (_bufferedWishDir - _smoothedWishDir) * _maxWishDirVel;
        _velocity = _velocity.ProcessLerp(desiredVelocity, _velDamping, dt);

        _smoothedWishDir += _velocity * dt;

        Position = _bodyMover.Body.GlobalPosition + _smoothedWishDir * _distance;
    }

    private Vector2 GetActiveTargetWishDir(float dt)
    {
        if (_bodyMover.CurrentWishDir != Vector2.Zero)
        {
            _acc = 0f;
            return _bodyMover.CurrentWishDir;
        }

        if (_acc < _outDelay)
            return _bufferedWishDir;

        Toggle(false);
        return Vector2.Zero;
    }

    private Vector2 GetUnactiveTargetWishDir(float dt)
    {
        if (_bodyMover.CurrentWishDir == Vector2.Zero)
        {
            _acc = 0f;
            return Vector2.Zero;
        }

        if (_acc < _inDelay)
            return Vector2.Zero;

        Toggle(true);
        return _bodyMover.CurrentWishDir;
    }

    private void Toggle(bool active)
    {
        _active = active;
        _acc = 0f;
    }
}
