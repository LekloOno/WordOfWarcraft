using Godot;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Combat.Abilities.Actuation.Drivers.Player;
using WowGd.Src.Dactylo.Generators;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Render.Animation.TweenTools;

namespace WowGd.Src.Render.Ui.Input.Impls.Worder;

[GlobalClass]
public partial class WorderDisplay : Node, IPlayerDactyloDriverHandler, IWordHandler
{
	[Export] private HFlowContainer _wordsContainer = null!;
	[Export] private Control _caret = null!;
	[Export] private TweenSettings _caretTweenSettings = null!;
	[Export] private TweenSettings _lineFadeSettings = null!;
	[Export] private TweenSettings _returnOffsetSettings = null!;
	private WordDisplay? _current;
	private int _idx = 0;

	public PlayerDactyloDriver Driver = null!;

	private Vector2 _targetCaretPosition;
	private Tween? _caretTween;
	private Tween? _lineFadeTween;
	private Tween? _returnOffsetTween;
	private int _lastCursorIdx = -1;

	public override void _Ready()
	{
		_caret.TopLevel = true;
		_wordsContainer.SortChildren += OnContainerSorted;
		this.Bind(Driver);
		ReadyHandInput();
	}

	private void OnContainerSorted()
	{
		UpdateCursor(_lastCursorIdx);
	}

	public void OnCharHit(char @char, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnCharMissed(char @char, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnCharMixed(char @char, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnCompleted(string written, string target, int correct) { }

	public void OnEraseAllHit(int count, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseAllMissed(int count, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseAllMixed(int count, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseHit(char @char, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseMissed(char @char, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnEraseMixed(char @char, int idx)
	{
		_lastCursorIdx = idx;
		UpdateCursor(idx);
	}

	public void OnWordEnqueued(Word word)
	{
		WordDisplay wd = new(word);
		wd.UpdateDisplay();
		_wordsContainer.AddChild(wd);
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

			Vector2 containerOffset = _wordsContainer.OffsetTransformPosition;
			containerOffset.Y = value;
			_wordsContainer.OffsetTransformPosition = containerOffset;

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
		if (_returnOffsetTween != null)
		{
			LineFaded();
			_returnOffsetTween.Finished -= LineFaded;
			_returnOffsetTween.Kill();
		}

		_fadingLine = BuildLine(yPos, out float nextYPos, out int fadedChildren);
		ControlNode.AddChild(_fadingLine);
		ControlNode.MoveChild(_fadingLine, 0);

		_idx -= fadedChildren;
		LineOffset = nextYPos - yPos;
		_fadingLineBaseOffset = yPos - nextYPos;

		_lineFadeTween?.Kill();
		_lineFadeTween = CreateTween();

		_lineFadeSettings.TweenProperty(_lineFadeTween, _fadingLine, 0f, "modulate:a");

		_returnOffsetTween = CreateTween();
		_returnOffsetTween.Finished += LineFaded;


		_returnOffsetSettings.TweenProperty(_returnOffsetTween, this, 0, nameof(LineOffset));
	}

	private void LineFaded()
	{
		_lineFadeTween?.Kill();
		_fadingLine?.QueueFree();
		_fadingLine = null;
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
			SizeFlagsHorizontal = _wordsContainer.SizeFlagsHorizontal,
			SizeFlagsVertical = _wordsContainer.SizeFlagsVertical,
			CustomMinimumSize = _wordsContainer.CustomMinimumSize,
		};

		foreach (Node node in _wordsContainer.GetChildren())
		{
			if (node is not WordDisplay wd || (nextYpos = wd.GlobalPosition.Y) != yPos)
				return fadingLine;

			wd.Reparent(fadingLine);
			fadedChildren++;
		}
		return fadingLine;
	}

	public void OnWordsInitialized(WordRequest req, Word[] words)
	{
		if (_current != null)
			this.Unbind(_current.Word);

		_current = null;

		_idx = 0;

		foreach (Word word in words)
			OnWordPushed(req, word);

		StartNext();
	}

	private void StartNext()
	{
		if (_current != null)
			this.Unbind(_current.Word);

		_current = _wordsContainer.GetChild(_idx) as WordDisplay;

		if (_current != null)
			this.Bind(_current.Word);

		UpdateCursor(-1);

		float yPos;
		if (_wordsContainer.GetChild(0) is Control control &&
			(yPos = control.GlobalPosition.Y) != _current?.GlobalPosition.Y)
			FadeLine(yPos);

		_idx++;
	}


	public void OnWordPushed(WordRequest req, Word word)
	{
		WordDisplay wd = new(word);
		wd.UpdateDisplay();
		_wordsContainer.AddChild(wd);
	}

	public void OnWordCompleted(WordRequest req, Word word)
	{
		StartNext();
	}

	public void OnStopped()
	{
		if (_current != null)
			this.Unbind(_current.Word);

		_current = null;

		foreach (Node node in _wordsContainer.GetChildren())
			node.QueueFree();

		_idx = 0;
	}

	public void OnCompleted() { }
}
