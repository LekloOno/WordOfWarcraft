using Godot;
using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui.Movement;

[GlobalClass]
public partial class TackleUi : Node, IDynamicTackleNodeHandler
{
    [Export] private Control _container = null!;
    [Export] private ProgressBar _body = null!;
    [Export] private TweenSettings _bodyTweenSettings = null!;
    [Export] private TweenSettings _showTweenSettings = null!;
    [Export] private TweenSettings _hideTweenSettings = null!;

    private Tween? _bodyTween;
    private Tween? _fadeTween;

    private float _defaultInterval = 0.25f;

    private bool _following;

    private bool _hasTick;
    private bool _hasInterval;
    private double _lastTickTime;
    private double _interval;

    private double _target = 1f;
    private double _segFrom, _segTo;
    private double _segStart, _segDuration;

    public override void _Ready()
    {
        _body.Value = 0f;
        Color mod = _container.Modulate;
        mod.A = 0f;
        _container.Modulate = mod;
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        if (!_following) return;
        double t = _segDuration <= 0 ? 1.0 : (Now - _segStart) / _segDuration;
        _body.Value = Mathf.Lerp(_segFrom, _segTo, (float)Mathf.Clamp(t, 0.0, 1.0));
    }

    public void OnGotReleased(DynamicTackledEventArgs tackleArgs) { }
    public void OnGotTackled(DynamicTackledEventArgs tackleArgs) { }

    public void OnTackleReleased(DynamicTackleNode tackleNode)
    {
        _following = false;
        SetProcess(false);

        _fadeTween?.Kill();
        _fadeTween = CreateTween();

        _hideTweenSettings.TweenProperty(_fadeTween, _container, 0f, "modulate:a");

        _bodyTween?.Kill();
        _bodyTween = CreateTween();

        _bodyTweenSettings.TweenProperty(_bodyTween, _body, 0f, "value");
    }

    public void OnTackleStarted(DynamicTackleNode tackleNode)
    {
        _bodyTween?.Kill();
        _fadeTween?.Kill();

        _following = false;
        SetProcess(false);
        _hasTick = false;
        _hasInterval = false;
        _target = 1f;

        _fadeTween = CreateTween();
        _showTweenSettings.TweenProperty(_fadeTween, _container, 1f, "modulate:a");

        _body.Value = 0f;
        _bodyTween = CreateTween();
        _bodyTweenSettings.TweenProperty(_bodyTween, _body, 1f, "value");
        _bodyTween.Finished += () =>
        {
            BeginSegment((float)_body.Value);
            _following = true;
            SetProcess(true);
        };
    }

    private static double Now => Time.GetTicksMsec() / 1000.0;
    public void OnStaminaTicked(double prev, double next)
    {
        double now = Now;
        if (_hasTick)
        {
            double dt = Mathf.Clamp(now - _lastTickTime, 0.02, 1.0);
            _interval = _hasInterval ? Mathf.Lerp(_interval, dt, 0.3) : dt;
            _hasInterval = true;
        }
        _hasTick = true;
        _lastTickTime = now;
        _target = next;

        if (_following)
            BeginSegment((float)_body.Value);
    }

    private void BeginSegment(float from)
    {
        _segFrom = from;
        _segTo = _target;
        _segStart = Now;
        _segDuration = _hasInterval ? _interval : _defaultInterval;
    }
}