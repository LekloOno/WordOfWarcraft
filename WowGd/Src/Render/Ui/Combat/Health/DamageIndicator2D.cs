using System;
using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui.Combat.Health;

[GlobalClass]
public partial class DamageIndicator2D : Label, IDamageIndicator
{
    [Export] private Vector2        _startScale;
    [Export] private Vector2        _randomOffsetRange = new (0.5f, 0.5f);
    [Export] private float          _gravity = 9.81f;
    [Export] private float          _launchStrength = 3.5f;
    [Export] private float          _launchAngleRange = 100f;
    [Export] private TweenSettings  _scaleInSettings  = null!;
    [Export] private TweenSettings  _fadeInSettings   = null!;
    [Export] private TweenSettings  _rotateInSettings = null!;
    [Export] private float _holdDelay = 0.5f;
    [Export] private TweenSettings  _fadeOutSettings  = null!;

    [Export] private DamageIndicatorColor _allyColors = null!;
    [Export] private DamageIndicatorColor _selfColors = null!;
    [Export] private DamageIndicatorColor _enemyColors = null!;

    private DamageIndicatorColor _activeColors = null!;

    [Export] private Curve _scaleCurve = null!;
    [Export] private float _deadScale       = 1f;
    [Export] private float _resurrectScale  = 1f;

    private Tween? _scaleTween;
    private Tween? _fadeTween;
    private Tween? _rotateTween;

    private Vector3 _worldPosition;

    public void OnDamaged(int hp)
    {
        Text = $"{hp}";
        Modulate = _activeColors.DamageColor;
        StartAnimation(_scaleCurve.Sample(hp));
    }

    public void OnDied()
    {
        Text = "Dead";
        Modulate = _activeColors.DeadColor;
        StartAnimation(_deadScale);
    }

    public void OnHealed(int hp)
    {
        Text = $"{hp}";
        Modulate = _activeColors.HealColor;
        StartAnimation(_scaleCurve.Sample(hp));
    }

    public void OnResurrected(int hp)
    {
        Text = "Resurrected";
        Modulate = _activeColors.ResColor;
        StartAnimation(_resurrectScale);
    }

    public override void _Process(double delta)
    {
        Camera3D camera = GetViewport().GetCamera3D();
        Visible = !camera.IsPositionBehind(_worldPosition);
        Position = camera.UnprojectPosition(_worldPosition);
        Position -= Size/2f;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float) delta;

        _velocity -= Vector2.Down * _gravity * dt;

        _worldPosition.X += _velocity.X * dt;
        _worldPosition.Y += _velocity.Y * dt;
    }

    private Vector2 _velocity;

    private void StartAnimation(float targetScale)
    {
        _velocity = InitVelocity();
        
        Scale = _startScale;
        
        Color selfMod = SelfModulate;
        selfMod.A = 0f;
        SelfModulate = selfMod;

        _scaleTween?.Kill();
        _fadeTween?.Kill();
        _rotateTween?.Kill();

        _scaleTween     = CreateTween();
        _fadeTween      = CreateTween();
        _rotateTween    = CreateTween();

        float rotDir = Random.Shared.Next(3) - 1;
        float rot = (float)_rotateInSettings.Value!.Value * rotDir;

        _scaleInSettings.TweenProperty( _scaleTween,    this, propertyPath: "scale", value: Vector2.One * targetScale);
        _fadeInSettings.TweenProperty(  _fadeTween,     this, propertyPath: "self_modulate:a");
        _rotateInSettings.TweenProperty(_rotateTween,   this, propertyPath: "rotation_degrees", value: rot);

        _fadeTween.TweenInterval(_holdDelay);
        
        _fadeOutSettings.TweenProperty( _fadeTween,     this, propertyPath: "self_modulate:a");

        _fadeTween.Finished += QueueFree;
    }

    private Vector2 InitVelocity()
    {
        if (_launchStrength == 0f)
            return Vector2.Zero;

        if (_launchAngleRange == 0f)
            return Vector2.Up * _launchStrength;
        
        float angle = (Random.Shared.NextSingle() - 0.5f) * _launchAngleRange;
        angle = Mathf.DegToRad(angle);

        return new Vector2(
            Mathf.Sin(angle) * _launchStrength,
            Mathf.Cos(angle) * _launchStrength
        );
    }

    public void SetWorldPosition(Vector3 worldPosition)
    {
        _worldPosition = worldPosition;

        _worldPosition.X += (Random.Shared.NextSingle() - 0.5f) * _randomOffsetRange.X;
        _worldPosition.Y += (Random.Shared.NextSingle() - 0.5f) * _randomOffsetRange.Y;
    }

    public void SetClientRelation(TargetRelation relation)
    {
        _activeColors = relation switch
        {
            TargetRelation.Self => _selfColors,
            TargetRelation.Ally => _allyColors,
            TargetRelation.Enemy => _enemyColors,
            _ => _enemyColors,
        };
    }
}