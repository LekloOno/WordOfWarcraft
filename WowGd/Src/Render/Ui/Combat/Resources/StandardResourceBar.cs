using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Combat.Resources.FocusRes;

namespace WowGd.Src.Render.Ui.Combat.Health;

[GlobalClass]
public partial class StandardResourceBar : Control, IStandardResourceHandler
{
    [Export] private ProgressBar _body = null!;
    [Export] private ProgressBar _tail = null!;
    [Export] private float _tailSpeed;
    [Export] private Tween.TransitionType _tailAnimation;

    private Tween? _tailTween;
    private IStandardResource _resource = null!;

    public override void _Ready()
    {
        if (_resource != null)
            Init();
    }

    private void Init()
    {
        _tailTween?.Kill();

        _body.MinValue = _tail.MinValue = 0f;
        _body.MaxValue = _tail.MaxValue = _resource.Max;
        _body.Value = _tail.Value = _resource.Current;
    }

    public void SetResource(IStandardResource resource)
    {
        if (_resource == resource)
            return;

        if (_resource != null)
            this.Unbind(_resource);

        _resource = resource;
        this.Bind(_resource);

        _tailTween?.Kill();

        if (_body is not null && _tail is not null)
            Init();
    }

    public void Consume(float currentHealth)
    {
        _body.Value = currentHealth;

        _tailTween?.Kill();
        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _body.Value, _tailSpeed).SetTrans(_tailAnimation);
    }

    public void Generate(float currentHealth)
    {
        _body.Value = currentHealth;

        _tailTween?.Kill();

        _tail.Value = Mathf.Max(_tail.Value, _body.Value);

        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _body.Value, _tailSpeed).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
    }

    public void OnConsumed(int rp) =>
        Consume(_resource.Current);

    public void OnGenerated(int rp) =>
        Generate(_resource.Current);
}