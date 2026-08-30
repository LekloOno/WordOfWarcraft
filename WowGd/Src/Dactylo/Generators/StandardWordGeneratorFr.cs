using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;

namespace WowGd.Src.Dactylo.Generators;

public class StandardWordGeneratorFr : IWordGenerator
{
    private const string WordBankFrPath = "res://assets/dactylo/word_bank_fr";
    public readonly static StandardWordGeneratorFr Instance = new();
    private readonly string[] _wordsContent = [];

    public StandardWordGeneratorFr()
    {
        _wordsContent = Extract(WordBankFrPath);
    }

    private string[] Extract(string bankPath)
    {
        if (!FileAccess.FileExists(bankPath))
        {
            GD.PushError($"Word bank not found: {bankPath}");
            return [];
        }

        using var file = FileAccess.Open(bankPath, FileAccess.ModeFlags.Read);
        var jsonText = file.GetAsText();
        return [.. jsonText.Split("|").OrderBy((s) => s.Length)];
    }

    public bool TryGenerate([NotNullWhen(true)] out Word? word, int minSize = 0, int maxSize = int.MaxValue)
    {
        string[] subBank = GetSubBank(minSize, maxSize);
        
        if (subBank.IsEmpty())
        {
            word = null;
            return false;
        }

        int idx = Random.Shared.Next(subBank.Length);
        word = new(subBank[idx]);
        return true;
    }

    public int TryGenerate(out Queue<Word> words, int count, int minSize = 0, int maxSize = int.MaxValue)
    {
        words = [];
        string[] subBank = GetSubBank(minSize, maxSize);
        
        if (subBank.IsEmpty())
            return 0;

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Shared.Next(subBank.Length);
            Word word = new(subBank[idx]);
            words.Enqueue(word);
        }

        return count;
    }

    public int TryGenerate(out Word[] words, int count, int minSize = 0, int maxSize = int.MaxValue)
    {
        words = new Word[count];
        string[] subBank = GetSubBank(minSize, maxSize);
        
        if (subBank.IsEmpty())
            return 0;

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Shared.Next(subBank.Length);
            words[i] = new(subBank[idx]);;
        }

        return count;
    }

    private string[] GetSubBank(int minSize, int maxSize) =>
        _wordsContent[FindFirstMatchingMinSize(minSize)..FindLastMatchingMaxSize(maxSize)];

    private int FindFirstMatchingMinSize(int minSize)
    {
        if (_wordsContent.IsEmpty())
            return -1;

        if (_wordsContent[0].Length >= minSize)
            return 0;

        int lastIdx = _wordsContent.Length - 1;
        if (_wordsContent[lastIdx].Length < minSize)
            return -1;
        
        int a = 0;
        int c = lastIdx;
        int b;

        while (c - a > 1)
        {
            b = (a + c) / 2;
            string s = _wordsContent[b];

            if (s.Length >= minSize)
                c = b;
            else
                a = b;
        }

        return c;
    }

    private int FindLastMatchingMaxSize(int maxSize)
    {
        if (_wordsContent.IsEmpty())
            return -1;

        if (_wordsContent[0].Length > maxSize)
            return -1;

        int lastIdx = _wordsContent.Length - 1;
        if (_wordsContent[lastIdx].Length <= maxSize)
            return lastIdx;
        
        int a = 0;
        int c = lastIdx;
        int b;

        while (c - a > 1)
        {
            b = (a + c) / 2;
            string s = _wordsContent[b];

            if (s.Length <= maxSize)
                a = b;
            else
                c = b;
        }

        return a;
    }
}