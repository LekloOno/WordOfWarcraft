using Godot;
using WowGd.Src.Combat.Health;

namespace WowGd.Src.Render.Ui.Combat.Resources.Bars;

[GlobalClass]
public partial class HealthBar : Control, IEntityHealthHandler
{
    [Export] private ProgressBar _body = null!;
    [Export] private ProgressBar _tail = null!;
    [Export] private float _tailSpeed;
    [Export] private Tween.TransitionType _tailAnimation;

    private Tween? _tailTween;
    private IEntityHealth _health = null!;

    public override void _Ready()
    {
        if (_health != null)
            Init();
    }

    private void Init()
    {
        _tailTween?.Kill();

        _body.MinValue = _tail.MinValue = 0f;
        _body.MaxValue = _tail.MaxValue = _health.Max;
        _body.Value = _tail.Value = _health.Current;
    }

    public void SetHealth(IEntityHealth health)
    {
        if (_health == health)
            return;

        if (_health != null)
            this.Unbind(_health);

        _health = health;
        this.Bind(_health);

        _tailTween?.Kill();

        if (_body is not null && _tail is not null)
            Init();
    }

    public void Damage(float currentHealth)
    {
        _body.Value = currentHealth;

        _tailTween?.Kill();
        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _body.Value, _tailSpeed).SetTrans(_tailAnimation);
    }

    public void Heal(float currentHealth)
    {
        _body.Value = currentHealth;

        _tailTween?.Kill();

        _tail.Value = Mathf.Max(_tail.Value, _body.Value);

        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _body.Value, _tailSpeed).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
    }

    public void OnDied() {}

    public void OnConsumed(int hp) =>
        Damage(_health.Current);

    public void OnGenerated(int hp) =>
        Heal(_health.Current);

    public void OnResurrected(int hp) =>
        Heal(_health.Current);
}