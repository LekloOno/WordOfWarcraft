using System;
using System.Globalization;
using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Render.Animation.TweenTools;
using WowGd.Src.Tools.Curve.Float;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators;

public abstract partial class ResIndicator2D<T> : Control, IResIndicator<T>
    where T: IResIndicatorColor
{
    [Export] private Label _back  = null!;
    [Export] private Label _shine = null!;
    [Export] private Label _fill  = null!;
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

    public abstract IResIndicatorRelationColors<T> RelationColors { get; }

    protected T _activeColors = default!;

    [Export] private FloatCurveSampler _scaleCurve = null!;

    private Tween? _scaleTween;
    private Tween? _fadeTween;
    private Tween? _rotateTween;

    private Vector3 _worldPosition;

    public void SetText(string text) =>
        _back.Text = _shine.Text = _fill.Text = text;
        
    public void SetValueText(int value) =>
        SetText($"{value.ToString("#,0", CultureInfo.GetCultureInfo("fr-FR"))}");

    public void OnConsumed(int rp)
    {
        SetValueText(-rp);
        Modulate = _activeColors.ConsumedColor;
        StartAnimation(_scaleCurve.Sample(rp));
    }

    public void OnGenerated(int rp)
    {
        SetValueText(rp);
        Modulate = _activeColors.GeneratedColor;
        StartAnimation(_scaleCurve.Sample(rp));
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

    protected void StartAnimation(float targetScale)
    {
        _velocity = InitVelocity();
        
        Scale = _startScale;
        
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;

        _scaleTween?.Kill();
        _fadeTween?.Kill();
        _rotateTween?.Kill();

        _scaleTween     = CreateTween();
        _fadeTween      = CreateTween();
        _rotateTween    = CreateTween();

        float rotDir = Random.Shared.Next(3) - 1;
        float rot = (float)_rotateInSettings.Value!.Value * rotDir;

        _scaleInSettings.TweenProperty( _scaleTween,    this, propertyPath: "scale", value: Vector2.One * targetScale);
        _fadeInSettings.TweenProperty(  _fadeTween,     this, propertyPath: "modulate:a");
        _rotateInSettings.TweenProperty(_rotateTween,   this, propertyPath: "rotation_degrees", value: rot);

        _fadeTween.TweenInterval(_holdDelay);
        
        _fadeOutSettings.TweenProperty( _fadeTween,     this, propertyPath: "modulate:a");

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

    public void SetClientRelation(TargetRelation relation) =>
        _activeColors = RelationColors.GetColor(relation);
}