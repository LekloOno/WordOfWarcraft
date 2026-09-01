using System;

namespace WowGd.Src.Dactylo.Generators;

public partial class Word
{
    public event Action<char, int>? CharHit;
    public event Action<char, int>? CharMixed;
    public event Action<char, int>? CharMissed;
    public event Action<char, int>? EraseHit;
    public event Action<char, int>? EraseMixed;
    public event Action<char, int>? EraseMissed;
    public event Action<int, int>? EraseAllHit;
    public event Action<int, int>? EraseAllMixed;
    public event Action<int, int>? EraseAllMissed;
    public event Action? Completed;

    private WordKeyOutcome ReturnNotifyHit(char @char)
    {
        CharHit?.Invoke(@char, Idx);
        return WordKeyOutcome.Hit;
    }

    private WordKeyOutcome ReturnNotifyMixed(char @char)
    {
        CharMixed?.Invoke(@char, Idx);
        return WordKeyOutcome.Mixed;
    }

    private WordKeyOutcome ReturnNotifyMissed(char @char)
    {
        CharMissed?.Invoke(@char, Idx);
        return WordKeyOutcome.Miss;
    }

    private WordKeyOutcome ReturnNotifyEraseHit(char @char)
    {
        EraseHit?.Invoke(@char, Idx);
        return WordKeyOutcome.Hit;
    }

    private WordKeyOutcome ReturnNotifyEraseMixed(char @char)
    {
        EraseMixed?.Invoke(@char, Idx);
        return WordKeyOutcome.Mixed;
    }

    private WordKeyOutcome ReturnNotifyEraseMissed(char @char)
    {
        EraseMissed?.Invoke(@char, Idx);
        return WordKeyOutcome.Miss;
    }

    private WordKeyOutcome ReturnNotifyEraseAllHit(int erased)
    {
        EraseAllHit?.Invoke(erased, Idx);
        return WordKeyOutcome.Hit;
    }

    private WordKeyOutcome ReturnNotifyEraseAllMixed(int erased)
    {
        EraseAllMixed?.Invoke(erased, Idx);
        return WordKeyOutcome.Mixed;
    }

    private WordKeyOutcome ReturnNotifyEraseAllMissed(int erased)
    {
        EraseAllMissed?.Invoke(erased, Idx);
        return WordKeyOutcome.Miss;
    }
}