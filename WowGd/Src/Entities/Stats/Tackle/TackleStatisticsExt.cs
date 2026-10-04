using System;
using Godot;
using WowGd.Src.Entities.Stats.Speed;

namespace WowGd.Src.Entities.Stats.Tackle;

/// <summary>
/// Provides utilities for converting and calculating entity tackle statistics.
/// </summary>
public static class TackleStatisticsExt
{
    /// <summary>
    /// A ratio used to adjust the distribution of the tackle weight.
    /// 
    /// <para>
    /// See s_scale in - https://www.desmos.com/calculator/cp5qp9vnq2
    /// </para>
    /// </summary>
    public const int RatioScale         = 25;
    /// <summary>
    /// The tackle weight upper bound, which implictly represents the
    /// maximum possible duration of a tackle in seconds.
    /// 
    /// <para>
    /// See m_max in - https://www.desmos.com/calculataor/cp5qp9vnq2
    /// </para>
    /// 
    /// <remarks>
    /// This value is approached as Tackle becomes increasingly greater
    /// than Dodge.
    /// The weight is interpreted as the projected tackle duration in
    /// seconds.
    /// </remarks>
    /// </summary>
    public const float MaxWeight        = 15f;
    /// <summary>
    /// The tackle weight lower bound, which implictly represents the
    /// minimum possible duration of a tackle in seconds.
    /// 
    /// <para>
    /// See m_min in - https://www.desmos.com/calculataor/cp5qp9vnq2
    /// </para>
    /// 
    /// <remarks>
    /// This value is approached as Tackle becomes increasingly lower
    /// than Dodge.
    /// The weight is interpreted as the projected tackle duration in
    /// seconds.
    /// </remarks>
    /// </summary>
    public const float MinWeight        = 0.5f;
    /// <summary>
    /// The tackle weight when Tackle and Dodge are equal.
    /// 
    /// <para>
    /// See y_0 in - https://www.desmos.com/calculataor/cp5qp9vnq2
    /// </para>
    /// 
    /// <remarks>
    /// This is the neutral point of the tackle-weight function.
    /// The weight is interpreted as the projected tackle duration in
    /// seconds.
    /// </remarks>
    /// </summary>
    public const float NeutralWeight    = 2.8f;
    /// <summary>
    /// The factor by which the tackler's max speed is multiplied by
    /// while tackling an entity.
    /// 
    /// <remarks>
    /// A value of <c>0.5</c> limits the tackler to 50% of their normal
    /// maximum speed while tackling.
    /// </remarks>
    /// </summary>
    public const float SpeedLimitFactor = 0.5f;

    /// <summary>
    /// Calculates the tackle weight for an interaction between two entities.
    /// 
    /// <para>
    /// See -
    /// https://www.desmos.com/calculator/cp5qp9vnq2
    /// </para>
    /// 
    /// <remarks>
    /// The tackle weight is determined from the difference between the tackling
    /// entity's Tackle statistic and the tackled entity's Dodge statistic.
    ///
    /// When Tackle and Dodge are equal, the result is <see cref="NeutralWeight"/>.
    /// A higher Tackle relative to Dodge produces a higher weight, while a lower
    /// Tackle relative to Dodge produces a lower weight.
    ///
    /// The result is bounded by <see cref="MinWeight"/> and
    /// <see cref="MaxWeight"/> as asymptotic limits.
    /// </remarks>
    /// </summary>
    /// <param name="tackling">The entity initiating the tackle.</param>
    /// <param name="tackled">The entity that is being tackled.</param>
    /// <returns>
    /// The calculated tackle weight, interpreted as the projected tackle
    /// duration in seconds.
    /// </returns>
    public static float GetTackleWeight(IEntity tackling, IEntity tackled)
    {
        int tackle  = StatEnum.Tackle.GetStat(tackling).Current;
        int dodge   = StatEnum.Dodge.GetStat(tackled).Current;

        float ratio = TackleRatio(tackle, dodge, RatioScale);
        return Weight(ratio, MaxWeight, MinWeight, NeutralWeight);
    }

    /// <summary>
    /// Gets the entity's effective tackle statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the tackle statistic.
    /// </param>
    /// <returns>
    /// The entity's effective tackle statistic.
    /// </returns>
    public static int Tackle(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Tackle).Current;

    /// <summary>
    /// Gets the entity's effective dodge statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the dodge statistic.
    /// </param>
    /// <returns>
    /// The entity's effective dodge statistic.
    /// </returns>
    public static int Dodge(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Dodge).Current;

    /// <summary>
    /// Gets the entity's effective speed statistic while tackling.
    /// 
    /// <remarks>
    /// The tackle speed is calculated by applying <see cref="SpeedLimitFactor"/>
    /// to the entity's current Speed statistic and rounding down to the nearest
    /// integer.
    /// </remarks>
    /// </summary>
    /// <param name="entity">The tackling entity.</param>
    /// <returns>The effective speed statistic this entity has during a tackle.</returns>
    public static int TackleSpeed(this IEntity entity) =>
        Mathf.FloorToInt(entity.Speed() * SpeedLimitFactor);

    /// <summary>
    /// Gets the entity's effective tackling speed in metres per second.
    /// </summary>
    /// <param name="entity">The tackling entity.</param>
    /// <returns>The effective tackling speed in metres per second.</returns>
    public static float TackleSpeedMs(this IEntity entity) =>
        SpeedStatisticsExt.GetMsFromSpeed(entity.TackleSpeed());

    private static float TackleRatio(int tackle, int dodge, int scale) =>
        (float)(tackle - dodge) / (Math.Abs(tackle) + Math.Abs(dodge) + scale);

    private static float Weight(float ratio, float max, float min, float neutral)
    {
        if (ratio >= 0f)
            return (ratio * (max - neutral)) + neutral;
        return ratio * (neutral - min) + neutral;
    }
}