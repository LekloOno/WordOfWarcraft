namespace WowGd.Src.Entities.Stats.Vitality;

/// <summary>
/// Provides utilities for converting and calculating entity vitality statistics.
/// </summary>
public static class VitalityStatisticsExt
{
    /// <summary>
    /// Gets the entity's effective vitality statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the vitality statistic.
    /// </param>
    /// <returns>
    /// The entity's effective vitality statistic.
    /// </returns>
    public static int Vitality(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Vitality).Current;

    /// <summary>
    /// Gets the entity's base vitality statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the base vitality statistic.
    /// </param>
    /// <returns>
    /// The entity's effective base vitality statistic.
    /// </returns>
    public static int VitalityBase(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Vitality).Base;
}