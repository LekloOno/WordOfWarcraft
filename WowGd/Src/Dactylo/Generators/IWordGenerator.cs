using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WowGd.Src.Dactylo.Generators;

public interface IWordGenerator
{
    bool TryGenerate([NotNullWhen(true)] out Word? word, int minSize = 0, int maxSize = int.MaxValue);
    int TryGenerate(out Queue<Word> words, int count, int minSize = 0, int maxSize = int.MaxValue);
    int TryGenerate(out Word[] words, int count, int minSize = 0, int maxSize = int.MaxValue);
}