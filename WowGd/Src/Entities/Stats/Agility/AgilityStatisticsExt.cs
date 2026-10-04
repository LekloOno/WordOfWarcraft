namespace WowGd.Src.Entities.Stats.Agility;

/// <summary>
/// Provides utilities for converting and calculating entity agility statistics.
/// </summary>
public static class AgilityStatisticsExt
{
    /// <summary>
    /// Gets the entity's effective agility statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the agility statistic.
    /// </param>
    /// <returns>
    /// The entity's effective agility statistic.
    /// </returns>
    public static int Agility(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Agility).Current;

    /// <summary>
    /// Gets the entity's base agility statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the base agility statistic.
    /// </param>
    /// <returns>
    /// The entity's effective base agility statistic.
    /// </returns>
    public static int AgilityBase(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Agility).Base;
}