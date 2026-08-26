using Godot;
using WowGd.Src.Dactylo.Generators;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui;

public partial class WorderDisplay : Node, IWorderHandler
{
	[Export] private Worder _worder = null!;
	[Export] private HFlowContainer _container = null!;
	[Export] private Control _caret = null!;
	[Export] private TweenSettings _caretTweenSettings = null!;
	[Export] private TweenSettings _lineFadeSettings = null!;
	[Export] private TweenSettings _returnOffsetSettings = null!;
	private WordDisplay? _current;
	private int _idx = 0;

	private Vector2 _targetCaretPosition;
	private Tween? _caretTween;
	private Tween? _lineFadeTween;
	private Tween? _returnOffsetTween;
	private int _lastCursorIdx = -1;

	public override void _Ready()
	{
		this.Bind(_worder);
		_caret.TopLevel = true;
		_container.SortChildren += OnContainerSorted;
	}

	private void OnContainerSorted()
	{
		UpdateCursor(_lastCursorIdx);
	}

	public void OnCharHit(char @char, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnCharMissed(char @char, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnCharMixed(char @char, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnCompleted(string written, string target, int correct)
	{
		_current?.UpdateDisplay();
	}

	public void OnEraseAllHit(int count, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseAllMissed(int count, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseAllMixed(int count, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseHit(char @char, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseMissed(char @char, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseMixed(char @char, int idx)
	{
		_current?.UpdateDisplay();
		_lastCursorIdx = idx;
		UpdateCursor(idx);
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
		UpdateCursor(-1);

		float yPos;
		if (_container.GetChild(0) is Control control &&
			(yPos = control.GlobalPosition.Y) != _current?.GlobalPosition.Y)
			FadeLine(yPos);

		_idx ++;
	}

	private void UpdateCursor(int idx)
	{
		if (_current == null)
			_targetCaretPosition = Vector2.Zero;
		else
			_targetCaretPosition = _current.GetCaretPosition();

		_caretTween?.Kill();

		if (_targetCaretPosition == _caret.GlobalPosition)
			return;

		_caretTween = CreateTween();



		_caretTweenSettings.TweenProperty(_caretTween, _caret, _targetCaretPosition, "global_position");
	}

	private float _lineOffset = 0f;
	public float LineOffset
	{
		get => _lineOffset;
		set
		{
			if (_lineOffset == value)
				return;

			_lineOffset = value;

			Vector2 caretOffset = _caret.OffsetTransformPosition;
			caretOffset.Y = value;
			_caret.OffsetTransformPosition = caretOffset;

			Vector2 containerOffset = _container.OffsetTransformPosition;
			containerOffset.Y = value;
			_container.OffsetTransformPosition = containerOffset;

			if (_fadingLine == null)
				return;

			Vector2 fadingLineOffset = _fadingLine.OffsetTransformPosition;
			fadingLineOffset.Y = value + _fadingLineBaseOffset;
			_fadingLine.OffsetTransformPosition = fadingLineOffset; 
		}
	}

	private HBoxContainer? _fadingLine;
	private float _fadingLineBaseOffset;

	private void FadeLine(float yPos)
	{
		_fadingLine = BuildLine(yPos, out float nextYPos, out int fadedChildren);
		AddChild(_fadingLine);
		MoveChild(_fadingLine, 0);

		_idx -= fadedChildren;
		LineOffset = nextYPos - yPos;
		_fadingLineBaseOffset = yPos - nextYPos;

		_lineFadeTween?.Kill();
		_lineFadeTween = CreateTween();

		_lineFadeSettings.TweenProperty(_lineFadeTween, _fadingLine, 0f, "modulate:a");

		if (_returnOffsetTween != null)
		{
			_returnOffsetTween.Finished -= LineFaded;
			_returnOffsetTween.Kill();
		}

		_returnOffsetTween = CreateTween();
		_returnOffsetTween.Finished += LineFaded;

		
		_returnOffsetSettings.TweenProperty(_returnOffsetTween, this, 0, nameof(LineOffset));
	}

	private void LineFaded()
	{
		_lineFadeTween?.Kill();
		_fadingLine?.QueueFree();
		LineOffset = 0;
	}

	private HBoxContainer BuildLine(float yPos, out float nextYpos, out int fadedChildren)
	{
		nextYpos = 0;
		fadedChildren = 0;
		
		HBoxContainer fadingLine = new()
		{
			OffsetTransformEnabled = true,
			Alignment = BoxContainer.AlignmentMode.Center,
		};

		foreach (Node node in _container.GetChildren())
		{
			if (node is not WordDisplay wd || (nextYpos = wd.GlobalPosition.Y) != yPos)
				return fadingLine;
			
			wd.Reparent(fadingLine);
			fadedChildren ++;
		}
		return fadingLine;
	}
}
