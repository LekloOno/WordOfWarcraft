using System;

namespace WowGd.Src.Combat.Abilities.Actuation;

public readonly struct TypingPackage
{
    public readonly int CorrectCharacters;
    public readonly float SentAccuracy;
    public readonly float TotalAccuracy;

    public TypingPackage(int correct, float sentAcc, float totalAcc)
    {
        CorrectCharacters = correct;
        SentAccuracy = sentAcc;
        TotalAccuracy = totalAcc;
    }

    public TypingPackage(int correct, int targetLength, int typed, int strokes)
    {
        CorrectCharacters = correct;

        SentAccuracy = (float)correct / Math.Max(targetLength, typed);
        TotalAccuracy = (float)correct / Math.Max(targetLength, strokes);
    }
}