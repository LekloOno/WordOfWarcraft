namespace WowGd.Src.Entities.Stats.Focus;

/// <summary>
/// Provides utilities for converting and calculating entity focus statistics.
/// </summary>
public static class FocusStatisticsExt
{
    /// <summary>
    /// Gets the entity's effective focus statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the focus statistic.
    /// </param>
    /// <returns>
    /// The entity's effective focus statistic.
    /// </returns>
    public static int Focus(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Focus).Current;

    /// <summary>
    /// Gets the entity's base focus statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the base focus statistic.
    /// </param>
    /// <returns>
    /// The entity's effective base focus statistic.
    /// </returns>
    public static int FocusBase(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Focus).Base;
}