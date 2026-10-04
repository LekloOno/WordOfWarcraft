namespace WowGd.Src.Entities.Stats.Grip;

/// <summary>
/// Provides utilities for converting and calculating entity grip statistics.
/// </summary>
public static class GripStatisticsExt
{
    /// <summary>
    /// Gets the entity's effective grip statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the grip statistic.
    /// </param>
    /// <returns>
    /// The entity's effective grip statistic.
    /// </returns>
    public static int Grip(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Grip).Current;

    /// <summary>
    /// Gets the entity's base grip statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the base grip statistic.
    /// </param>
    /// <returns>
    /// The entity's effective base grip statistic.
    /// </returns>
    public static int GripBase(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Grip).Base;
}