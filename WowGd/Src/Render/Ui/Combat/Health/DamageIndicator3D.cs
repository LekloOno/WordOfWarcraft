using Godot;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui.Combat.Health;

[GlobalClass]
public partial class DamageIndicator3D : Label3D, IDamageIndicator
{
    [Export] private Vector3        _startScale;
    [Export] private TweenSettings  _scaleInSettings  = null!;
    [Export] private TweenSettings  _fadeInSettings   = null!;
    [Export] private TweenSettings  _rotateInSettings = null!;
    [Export] private float _holdDelay = 0.5f;
    [Export] private TweenSettings  _scaleOutSettings = null!;
    [Export] private TweenSettings  _fadeOutSettings  = null!;

    [Export] private Color _damageColor;
    [Export] private Color _healColor;
    [Export] private Color _resColor;
    [Export] private Color _deadColor;

    private Tween? _scaleTween;
    private Tween? _fadeTween;
    private Tween? _rotateTween;

    public void OnDamaged(int hp)
    {
        Text = $"{hp}";
        Modulate = _damageColor;
        StartAnimation();
    }

    public void OnDied()
    {
        Text = "Dead";
        Modulate = _deadColor;
        StartAnimation();
    }

    public void OnHealed(int hp)
    {
        Text = $"{hp}";
        Modulate = _healColor;
        StartAnimation();
    }

    public void OnResurrected(int hp)
    {
        Text = $"{hp}";
        Modulate = _resColor;
        StartAnimation();
    }

    public float RotationZ;

    public override void _Process(double delta)
    {
        LookAt(GetViewport().GetCamera3D().GlobalPosition);

        RotateY(Mathf.Pi);
    }

    private float _opacity = 0f;
    public float Opacity
    {
        get => _opacity;
        set
        {
            if (_opacity == value)
                return;

            _opacity = value;

            Modulate = new Color(
                Modulate.R,
                Modulate.G,
                Modulate.B,
                _opacity
            );

            OutlineModulate = new Color(
                OutlineModulate.R,
                OutlineModulate.G,
                OutlineModulate.B,
                _opacity
            );
        }
    }

    private void StartAnimation()
    {
        Scale = _startScale;
        
        Opacity = 0f;

        _scaleTween?.Kill();
        _fadeTween?.Kill();
        _rotateTween?.Kill();

        _scaleTween     = CreateTween();
        _fadeTween      = CreateTween();
        _rotateTween    = CreateTween();

        _scaleInSettings.TweenProperty( _scaleTween,    this, propertyPath: "scale");
        _fadeInSettings.TweenProperty(  _fadeTween,     this, propertyPath: nameof(Opacity));
        _rotateInSettings.TweenProperty(_rotateTween,   this, propertyPath: nameof(RotationZ));

        _scaleTween.TweenInterval(_holdDelay);
        _fadeTween.TweenInterval(_holdDelay);
        
        _scaleOutSettings.TweenProperty(_scaleTween,    this, propertyPath: "scale");
        _fadeOutSettings.TweenProperty( _fadeTween,     this, propertyPath: nameof(Opacity));

        _fadeTween.Finished += QueueFree;
    }

    public void SetWorldPosition(Vector3 worldPosition)
    {
        TopLevel = true;
        GlobalPosition = worldPosition;
    }
}