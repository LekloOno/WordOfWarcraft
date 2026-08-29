using System;
using Godot;

namespace WowGd.Src.Combat.Abilities.Exp.Data.Resources.CasterPreconditions.ResourceChecks;

public abstract partial class CasterResourcePreconditionData : CasterPrecondition
{
    [Export] public float               Amount      { get; private set; }
    [Export] public ResourceAmountType  AmountType  { get; private set; }
    [Export] public ResourceComparison  Comparison  { get; private set; }
}

public enum ResourceAmountType
{
    Flat,
    BasePercent,    // Base maximum with no modifiers
    MaxPercent,     // Absolute maximum
}


public enum ResourceComparison
{
    Less,
    Equal,
    More,
}

public static class CasterResourcePreconditionDataExt
{
    public static bool Compare(
        this CasterResourcePreconditionData data,
        float current,
        float maxTotal,
        float @base)
    {
        return data.AmountType switch
        {
            ResourceAmountType.Flat => data.Comparison.Compare(current, data.Amount),
            ResourceAmountType.MaxPercent => data.Comparison.Compare(current/maxTotal, data.Amount),
            ResourceAmountType.BasePercent => data.Comparison.Compare(current/@base, data.Amount),

            _ => throw new ArgumentOutOfRangeException(nameof(data.AmountType))
        };
    }

    private static bool Compare(
        this ResourceComparison comparison,
        float right,
        float left)
    {
        return comparison switch
        {
            ResourceComparison.Less  => left < right,
            ResourceComparison.Equal => left == right,
            ResourceComparison.More  => left > right,

            _ => throw new ArgumentOutOfRangeException(nameof(comparison))
        };
    }
}