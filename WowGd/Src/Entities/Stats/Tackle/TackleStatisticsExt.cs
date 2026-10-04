using System;
using WowGd.Src.Entities.Stats.Modifier;

namespace WowGd.Src.Entities.Stats.Tackle;

public static class TackleStatisticsExt
{
    public const int RatioScale      = 25;
    public const float MaxWeight     = 15f;
    public const float MinWeight     = 0.5f;
    public const float NeutralWeight = 2.8f;

    /// <summary>
    /// https://www.desmos.com/calculator/cp5qp9vnq2
    /// </summary>
    /// <param name="tackling"></param>
    /// <param name="tackled"></param>
    /// <returns></returns>
    public static float GetTackleWeight(IEntity tackling, IEntity tackled)
    {
        int tackle  = TargetStat.Tackle.GetStat(tackling).Current;
        int dodge   = TargetStat.Dodge.GetStat(tackled).Current;

        float ratio = TackleRatio(tackle, dodge, RatioScale);
        return Weight(ratio, MaxWeight, MinWeight, NeutralWeight);
    }

    private static float TackleRatio(int tackle, int dodge, int scale) =>
        (float)(tackle - dodge) / (Math.Abs(tackle) + Math.Abs(dodge) + scale);

    private static float Weight(float ratio, float max, float min, float neutral)
    {
        if (ratio >= 0f)
            return (ratio * (max - neutral)) + neutral;
        return ratio * (neutral - min) + neutral;
    }
}