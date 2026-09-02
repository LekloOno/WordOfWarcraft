using System;
using Godot;

namespace WowGd.Src.Combat.Abilities.Actuation;

public static class TypingPackageExt
{
    public static float GetWeight(this TypingPackage package, float perfectMultiplier, float accuracyMultiplier)
    {
        if (package.TotalAccuracy == 1f)
            return perfectMultiplier;

        if (accuracyMultiplier == 0f)
            return 1f;
        
        if (accuracyMultiplier == 0.5f)
            return package.SentAccuracy;

        if (accuracyMultiplier == 1f)
            return package.SentAccuracy == 1f ? 1f : 0f;

        return Mathf.Pow(package.SentAccuracy, -MathF.Log2(1-accuracyMultiplier));
    }
}