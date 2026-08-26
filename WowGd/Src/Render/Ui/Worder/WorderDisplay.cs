using System;
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
	private WordDisplay? _current;
	private int _idx = 0;

	private Vector2 _targetCaretPosition;
	private Tween? _caretTween;
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
		_idx ++;
		UpdateCursor(-1);
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
}
