using Godot;
using WowGd.Src.Dactylo.Generators;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui;

public partial class WorderDisplay : Node, IWorderHandler
{
	[Export] private Worder _worder = null!;
	[Export] private HFlowContainer _container = null!;
	[Export] private Control _cursor = null!;
	[Export] private TweenSettings _cursorTweenSettings = null!;
	private WordDisplay? _current;
	private int _idx = 0;

	private Vector2 _targetCursorPosition;
	private Tween? _cursorTween;

	public override void _Ready()
	{
		this.Bind(_worder);
	}

	public void OnCharHit(char @char, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnCharMissed(char @char, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnCharMixed(char @char, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnCompleted(string written, string target, int correct)
	{
		_current?.UpdateDisplay();
	}

	public void OnEraseAllHit(int count, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnEraseAllMissed(int count, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnEraseAllMixed(int count, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnEraseHit(char @char, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnEraseMissed(char @char, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnEraseMixed(char @char, int idx)
	{
		UpdateCursor(idx);
		_current?.UpdateDisplay();
	}

	public void OnWordEnqueued(Word word)
	{ 
		WordDisplay wd = new(word);
		wd.UpdateDisplay();
		_container.AddChild(wd);		
	}

	public void OnWordStarted(string word, int remainingWords)
	{
		_current = _container.GetChild(_idx) as WordDisplay;
		_idx ++;
		UpdateCursor(-1);
	}

	private void UpdateCursor(int idx)
	{
		if (_current == null)
			_targetCursorPosition = Vector2.Zero;
		else
			_targetCursorPosition = _current.Position + Vector2.Right * (idx + 1) * 9.5f;


		_cursorTween?.Kill();
		_cursorTween = CreateTween();



		_cursorTweenSettings.TweenProperty(_cursorTween, _cursor, _targetCursorPosition, "offset_transform_position");
	}
}
