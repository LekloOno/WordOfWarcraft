using System;
using Godot;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui.Combat.Health;

[GlobalClass]
public partial class DamageIndicator2D : Label, IDamageIndicator
{
    [Export] private Vector2        _startScale;
    [Export] private TweenSettings  _scaleInSettings  = null!;
    [Export] private TweenSettings  _fadeInSettings   = null!;
    [Export] private TweenSettings  _rotateInSettings = null!;
    [Export] private float _holdDelay = 0.5f;
    [Export] private TweenSettings  _fadeOutSettings  = null!;

    [Export] private Color _damageColor;
    [Export] private Color _healColor;
    [Export] private Color _resColor;
    [Export] private Color _deadColor;

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
        Modulate = _damageColor;
        StartAnimation(_scaleCurve.Sample(hp));
    }

    public void OnDied()
    {
        Text = "Dead";
        Modulate = _deadColor;
        StartAnimation(_deadScale);
    }

    public void OnHealed(int hp)
    {
        Text = $"{hp}";
        Modulate = _healColor;
        StartAnimation(_scaleCurve.Sample(hp));
    }

    public void OnResurrected(int hp)
    {
        Text = "Resurrected";
        Modulate = _resColor;
        StartAnimation(_resurrectScale);
    }

    public override void _Process(double delta)
    {
        Camera3D camera = GetViewport().GetCamera3D();
        Visible = !camera.IsPositionBehind(_worldPosition);
        Position = camera.UnprojectPosition(_worldPosition);
        Position -= Size/2f;
    }

    private void StartAnimation(float targetScale)
    {
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

    public void SetWorldPosition(Vector3 worldPosition)
    {
        _worldPosition = worldPosition;
    }
}