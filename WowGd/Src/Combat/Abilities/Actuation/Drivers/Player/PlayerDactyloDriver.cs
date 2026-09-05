using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using Godot;
using WowGd.Src.Dactylo.Generators;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Combat.Abilities.Actuation.Drivers;

[GlobalClass]
public partial class PlayerDactyloDriver : Node, IActuatorDriver, ITwoHandedInputMode
{
    private readonly Channel<InputEventKey> _inputChannel = Channel.CreateUnbounded<InputEventKey>();
    private readonly IWordGenerator _generator = StandardWordGeneratorFr.Instance;

    public event Action<WordRequest, Word[]>? WordsInitialized;
    public event Action<WordRequest, Word>? WordPushed;
    public event Action<WordRequest, Word>? WordCompleted;
    public event Action? Stopped;
    private bool _processing = false;

    public override void _Ready()
	{
        SetProcessUnhandledKeyInput(false);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.IsPressed())
            _inputChannel.Writer.TryWrite(key);
    }

    private void DrainInputChannel()
    {
        while (_inputChannel.Reader.TryRead(out _)) {}
    }

    public async IAsyncEnumerable<TypingPackage> StreamTypingAsync(WordRequest request, [EnumeratorCancellation] CancellationToken ct)
    {
        DrainInputChannel();

        try
        {
            _processing = true;
            HandsInputManager.TryPushTwoHandedMode(this);

            Queue<Word> wordQueue = new();
            int totalWordsToQueue = 1 + request.LookaheadCount;

            if (_generator.TryGenerate(out Word[] initialWords, totalWordsToQueue, request.MinLength, request.MaxLength) > 0)
            {
                foreach (Word w in initialWords) wordQueue.Enqueue(w);
                WordsInitialized?.Invoke(request, initialWords);
            }

            while (!ct.IsCancellationRequested)
            {
                Word currentWord = wordQueue.Dequeue();
                int totalStrokes = 0;

                await foreach (InputEventKey key in _inputChannel.Reader.ReadAllAsync(ct))
                {
                    if (key.Keycode == Key.Space)
                    {
                        currentWord.Complete();
                        if (request.Mode == DeliveryMode.PerWord)
                        {
                            WordCompleted?.Invoke(request, currentWord);
                            yield return CreatePackage(currentWord, totalStrokes);
                        }

                        break; 
                    }

                    totalStrokes++;
                    
                    WordKeyOutcome outcome = ProcessKey(key, currentWord);

                    if (outcome == WordKeyOutcome.Ignored) 
                        continue;

                    if (request.Mode == DeliveryMode.PerKeystroke)
                        yield return CreatePackage(currentWord, totalStrokes);
                }

                if (_generator.TryGenerate(out Word[] newWords, 1, request.MinLength, request.MaxLength) > 0)
                {
                    Word nextWord = newWords[0];
                    wordQueue.Enqueue(nextWord);
                    WordPushed?.Invoke(request, nextWord);
                }
            }
        }
        finally
        {
            _processing = false;
            HandsInputManager.TryRemoveTwoHandedMode(this);
            Stopped?.Invoke();
        }
    }

    private static WordKeyOutcome ProcessKey(InputEventKey key, Word word)
    {
        if (key.Keycode == Key.Backspace)
            return TryErase(key, word);
            
        if (key.Unicode != 0)
            return TryWrite(key, word);
            
        return WordKeyOutcome.Ignored;
    }

    private static WordKeyOutcome TryWrite(InputEventKey key, Word word)
    {
        char @char = (char)key.Unicode;
        return word.Write(@char);
    }

    private static WordKeyOutcome TryErase(InputEventKey key, Word word)
    {
        if ((key.GetModifiersMask() & KeyModifierMask.MaskCtrl) != 0)
            return word.EraseAll();

        return word.EraseOne(out _);
    }

    private static TypingPackage CreatePackage(Word word, int totalStrokes) =>
        new(word.Correct, word.Length, word.WrittenLength, totalStrokes);

    public bool CanStart() => _processing;
    public void Start()
    {
        if (_processing)
            SetProcessUnhandledKeyInput(true);
    }

    public bool CanStop() => true;

    public void Stop()
    {
        SetProcessUnhandledKeyInput(false);
    }

}