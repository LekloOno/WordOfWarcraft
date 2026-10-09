using System;
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

    [Export] private ProgressBar _tail = null!;
    [Export] private double _jumpDuration = 0.12;
    [Export] private double _tailHold = 0.35;
    [Export] private double _tailSpeed = 1.5;

    private double _offset, _tailValue, _tailTimer;

    private Tween? _bodyTween;
    private Tween? _tailTween;
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

    private double LineValue
    {
        get
        {
            double t = _segDuration <= 0 ? 1.0 : (Now - _segStart) / _segDuration;
            return Mathf.Lerp(_segFrom, _segTo, Mathf.Clamp(t, 0.0, 1.0));
        }
    }

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

        _offset = Mathf.MoveToward(_offset, 0.0, delta / _jumpDuration);
        double body = Mathf.Clamp(LineValue + _offset, 0.0, 1.0);

        if (_tailTimer > 0) _tailTimer -= delta;
        else _tailValue = Mathf.MoveToward(_tailValue, body, _tailSpeed * delta);
        _tailValue = Math.Max(_tailValue, body);

        _body.Value = body;
        _tail.Value = _tailValue;
    }

    public void OnGotReleased(DynamicTackledEventArgs tackleArgs) { }
    public void OnGotTackled(DynamicTackledEventArgs tackleArgs) { }

    public void OnTackleReleased(DynamicTackleNode tackleNode)
    {
        _bodyTween?.Kill();
        _tailTween?.Kill();
        _fadeTween?.Kill();

        _following = false;
        SetProcess(false);

        _fadeTween = CreateTween();

        _hideTweenSettings.TweenProperty(_fadeTween, _container, 0f, "modulate:a");

        _offset = 0;

        _tailTween = CreateTween();
        _bodyTweenSettings.TweenProperty(_tailTween, _tail, 0f, "value");

        _bodyTween = CreateTween();
        _bodyTweenSettings.TweenProperty(_bodyTween, _body, 0f, "value");
    }

    public void OnTackleStarted(DynamicTackleNode tackleNode)
    {
        _bodyTween?.Kill();
        _tailTween?.Kill();        
        _fadeTween?.Kill();

        _following = false;
        SetProcess(false);
        _hasTick = false;
        _hasInterval = false;
        _target = 1f;

        _fadeTween = CreateTween();
        _showTweenSettings.TweenProperty(_fadeTween, _container, 1f, "modulate:a");

        _body.Value = 0f;
        _offset = _tailTimer = 0;

        _bodyTween = CreateTween();
        _bodyTweenSettings.TweenProperty(_bodyTween, _body, 1f, "value");
        _bodyTween.Finished += () =>
        {
            BeginSegment(_body.Value);
            _tailValue = _body.Value;
            _following = true;
            SetProcess(true);
        };
    }

    private static double Now => Time.GetTicksMsec() / 1000.0;
    public void OnStaminaChanged(StaminaChange change)
    {
        _target = change.Value;

        if (change.Kind == StaminaChangeKind.Tick)
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

            if (_following)
                BeginSegment(LineValue);

            return;
        }

        if (!_following) return;

        double line = LineValue;
        double shown = Mathf.Clamp(line + _offset, 0.0, 1.0);
        double shifted = Mathf.Clamp(line + change.Delta, 0.0, 1.0);

        if (change.Kind == StaminaChangeKind.Drain)
        {
            _tailValue = Math.Max(_tailValue, shown);
            _tailTimer = _tailHold;
        }

        _offset += line - shifted;
        _segFrom = Mathf.Clamp(_segFrom + change.Delta, 0.0, 1.0);
        _segTo = change.Value;
    }

    private void BeginSegment(double from)
    {
        _segFrom = from;
        _segTo = _target;
        _segStart = Now;
        _segDuration = _hasInterval ? _interval : _defaultInterval;
    }
}