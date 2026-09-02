using System;

namespace WowGd.Src.Combat.Abilities.CasterRules.ResourceRules;

public static class CasterResourceRuleExt
{
    public static bool Compare(
        this CasterResourceRule data,
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