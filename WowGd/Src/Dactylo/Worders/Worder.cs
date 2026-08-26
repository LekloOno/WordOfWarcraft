using System;
using System.Collections.Generic;
using Godot;
using WowGd.Src.Dactylo.Generators;
using WowGd.Src.Tools;

namespace WowGd.Src.Dactylo.Worders;

[GlobalClass]
public partial class Worder : Node, IWorder
{
    [Export] private int _minSize = 1;
	[Export] private int _maxSize = 10;
    [Export] private int _preLoaded = 20;

    private readonly IWordGenerator _generator = StandardWordGeneratorFr.Instance;
    private readonly Queue<Word> _activeWords = [];
    private Word? _current;

    public event Action<char, int>? CharHit;
    public event Action<char, int>? CharMixed;
    public event Action<char, int>? CharMissed;
    public event Action<char, int>? EraseHit;
    public event Action<char, int>? EraseMixed;
    public event Action<char, int>? EraseMissed;
    public event Action<int, int>? EraseAllHit;
    public event Action<int, int>? EraseAllMixed;
    public event Action<int, int>? EraseAllMissed;
    public event Action<string, string, int>? Completed;
    public event Action<string, int>? WordStarted;
    public event Action<Word>? WordEnqueued;

    public string? CurrentWord => _current?.Content;
    public string? Written => _current?.Written;
    public int? CurrentCorrect => _current?.Correct;
    public int? CurrentLength => _current?.Length;
    public IReadOnlyCollection<Word> Words => _activeWords;

    public bool Enabled => _enabled;
    private bool _enabled = false;

    public int TryGenerate(int count, int minSize = 0, int maxSize = int.MaxValue)
    {
        int generated = _generator.TryGenerate(out Queue<Word> words, count, minSize, maxSize);
        
        foreach (Word word in words)
        {
            _activeWords.Enqueue(word);
            WordEnqueued?.Invoke(word);
        }

        return generated;
    }

    public bool Next()
    {
        if (_current != null)
        {
            GD.Print($"written - {_current.Written} against - {_current.Content}");
            Completed?.Invoke(_current.Written, _current.Content, _current.Correct);
        }

        if (_preLoaded != 0 && _generator.TryGenerate(out Word? newWord, _minSize, _maxSize))
        {
            _activeWords.Enqueue(newWord);
            WordEnqueued?.Invoke(newWord);
        }
            
        if(!_activeWords.TryDequeue(out Word? word))
        {
            _current = null;
            return false;
        }

        _current = word;
        GD.Print(_current.Content);
        WordStarted?.Invoke(word.Content, _activeWords.Count);

        return true;
    }

    public void Process(InputEventKey key)
    {
        if (key.Keycode == Key.Backspace)
            Erase(key);
        else if (key.Keycode == Key.Space)
            Next();
        else if (_current != null)
            TryWrite(key, _current);
    }

    private void TryWrite(InputEventKey key, Word word)
    {
        if (key.Unicode == 0)
            return;

        char @char = (char) key.Unicode;

        WordKeyOutcome outcome = word.Write(@char);
        NotifyWrite(outcome, @char, word.Idx);
    }

    private void Erase(InputEventKey key)
    {
        if (_current == null)
            return;

        if ((key.GetModifiersMask() & KeyModifierMask.MaskCtrl) != 0)
        {
            int count = _current.WrittenLength;
            WordKeyOutcome outcome = _current.EraseAll();
            NotifyEraseAll(outcome, count, _current.Idx);
        }
        else
        {
            WordKeyOutcome outcome = _current.EraseOne(out char @char);
            NotifyEraseOne(outcome, @char, _current.Idx);
        }
    }

    private void NotifyWrite(WordKeyOutcome outcome, char @char, int idx)
    {
        switch (outcome)
        {
            case WordKeyOutcome.Hit :
                CharHit?.Invoke(@char, idx);
                break;
            case WordKeyOutcome.Miss :
                CharMissed?.Invoke(@char, idx);
                break;
            case WordKeyOutcome.Mixed :
                CharMixed?.Invoke(@char, idx);
                break;
        }
    }

    private void NotifyEraseAll(WordKeyOutcome outcome, int count, int idx)
    {
        switch (outcome)
        {
            case WordKeyOutcome.Hit :
                EraseAllHit?.Invoke(count, idx);
                break;
            case WordKeyOutcome.Miss :
                EraseAllMissed?.Invoke(count, idx);
                break;
            case WordKeyOutcome.Mixed :
                EraseAllMixed?.Invoke(count, idx);
                break;
        }
    }

    private void NotifyEraseOne(WordKeyOutcome outcome, char @char, int idx)
    {
        switch (outcome)
        {
            case WordKeyOutcome.Hit :
                EraseHit?.Invoke(@char, idx);
                break;
            case WordKeyOutcome.Miss :
                EraseMissed?.Invoke(@char, idx);
                break;
            case WordKeyOutcome.Mixed :
                EraseMixed?.Invoke(@char, idx);
                break;
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key)
            return;

        if (!key.IsPressed())
            return;

        Process(key);
    }

    public override void _Ready()
    {
        TryGenerate(_preLoaded, _minSize, _maxSize);
        SetProcessUnhandledKeyInput(false);
    }

    public bool Enable() =>
        DisableExt.IndempEnable(ref _enabled, () => SetProcessUnhandledKeyInput(true));

    public bool Disable() =>
        DisableExt.IndempDisable(ref _enabled, () => SetProcessUnhandledKeyInput(false));
}