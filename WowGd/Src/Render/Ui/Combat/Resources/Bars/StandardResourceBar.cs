using Godot;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui.Combat.Resources.Bars;

[GlobalClass]
public partial class StandardResourceBar : Control, IStandardResourceHandler
{
	[Export] private ProgressBar _body = null!;
	[Export] private ProgressBar _tail = null!;
	// Null tween settings means snap to target value.
	[Export] private TweenSettings? _tailTweenSettings = null!;
	[Export] private TweenSettings? _bodyTweenSettings = null!;
	[Export] private bool _tweenBodyOnGrowthOnly = true;

	[ExportGroup("Label")]
	[Export] private Label? _label;
	[Export] private BarLabelFormat _labelFormat = BarLabelFormat.CurrentOverMax;

	private Tween? _tailTween;
	private Tween? _bodyTween;
	private IStandardResource? _resource;

	public override void _EnterTree() => _body.ValueChanged += OnBodyValueChanged;
	public override void _ExitTree()  => _body.ValueChanged -= OnBodyValueChanged;

	public override void _Ready()
	{
		if (_resource != null)
			Init();
	}

	public void SetResource(IStandardResource resource)
	{
		if (_resource == resource)
			return;

		if (_resource != null)
			UnbindResource(_resource);

		_resource = resource;

		if (_resource != null)
			BindResource(_resource);

		if (IsNodeReady() && _resource != null)
			Init();
	}

	protected virtual void BindResource(IStandardResource resource)   => this.Bind(resource);
	protected virtual void UnbindResource(IStandardResource resource) => this.Unbind(resource);

	private void Init()
	{
		_tailTween?.Kill();
		_bodyTween?.Kill();

		_body.MinValue = _tail.MinValue = 0f;
		_body.MaxValue = _tail.MaxValue = _resource!.Max;
		_body.Value = _tail.Value = _resource.Current;

		UpdateLabel();
	}

	private void UpdateLabel()
	{
		if (_label is null)
			return;

		_label.Text = FormatValue(_body.Value, _body.MaxValue);
	}

	private void OnBodyValueChanged(double value) => UpdateLabel();

	private void Refresh(bool growing)
	{
		if (_resource is null)
			return;

		float target = _resource.Current;

		TweenSettings? bodySettings = growing ? _bodyTweenSettings :
			(_tweenBodyOnGrowthOnly ? null : _bodyTweenSettings);

		SetLayer(_body, ref _bodyTween, bodySettings, target);

		if (growing)
			_tail.Value = Mathf.Max(_tail.Value, _body.Value);

		SetLayer(_tail, ref _tailTween, _tailTweenSettings, target);
	}

	protected virtual string FormatValue(double value, double max) => _labelFormat switch
	{
		BarLabelFormat.Current        => $"{Mathf.RoundToInt(value)}",
		BarLabelFormat.CurrentOverMax => $"{Mathf.RoundToInt(value)} / {Mathf.RoundToInt(max)}",
		BarLabelFormat.Percent        => max > 0 ? $"{Mathf.RoundToInt(value / max * 100)}%" : "0%",
		_                             => string.Empty,
	};

	private void SetLayer(ProgressBar bar, ref Tween? tween, TweenSettings? settings, float target)
	{
		tween?.Kill();
		tween = null;

		if (settings is null)
		{
			bar.Value = target;
			return;
		}

		tween = CreateTween();
		settings.TweenProperty(tween, bar, target, "value");
	}

	public void OnConsumed(int rp)  => Refresh(growing: false);
	public void OnGenerated(int rp) => Refresh(growing: true);

	public void OnMaxChanged(int max)
	{
		if (_resource is null)
			return;

		if (_body.MaxValue == max)
			return;

		_body.MaxValue = _tail.MaxValue = max;

		Refresh(growing: false);
		UpdateLabel();
	}
}
