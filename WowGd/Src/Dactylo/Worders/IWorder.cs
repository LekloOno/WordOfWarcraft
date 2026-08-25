using System;
using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Dactylo.Worders;

public interface IWorder : IDisablable
{
    string? CurrentWord { get; }
    string? Written     { get; }
    int? CurrentCorrect { get; }
    int? CurrentLength  { get; }

    event Action<char, int>? CharHit;
    event Action<char, int>? CharMixed;
    event Action<char, int>? CharMissed;
    
    event Action<char, int>? EraseHit;
    event Action<char, int>? EraseMixed;
    event Action<char, int>? EraseMissed;
    
    event Action<int, int>? EraseAllHit;
    event Action<int, int>? EraseAllMixed;
    event Action<int, int>? EraseAllMissed;

    event Action<string, string, int>? Completed;
    event Action<string, int>? WordStarted;

    void Process(InputEventKey key);
    int TryGenerate(int count, int minSize = 0, int maxSize = int.MaxValue);
    bool Next();
}