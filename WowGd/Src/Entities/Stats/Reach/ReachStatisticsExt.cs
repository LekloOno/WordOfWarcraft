namespace WowGd.Src.Entities.Stats.Reach;

/// <summary>
/// Provides utilities for converting and calculating entity reach statistics.
/// </summary>
public static class ReachStatisticsExt
{
    /// <summary>
    /// Metres represented by one reach statistic point.
    /// </summary>
    public const float ReachToMetresFactor = 0.1f;

    /// <summary>
    /// Converts the given reach statistic into a distance expressed in metres.
    /// </summary>
    /// <param name="reach">
    /// The reach statistic to convert.
    /// </param>
    /// <returns>
    /// The equivalent distance in metres.
    /// </returns>
    public static float GetMetresFromReach(int reach) => reach * ReachToMetresFactor;

    /// <summary>
    /// Gets the entity's effective reach statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the reach statistic.
    /// </param>
    /// <returns>
    /// The entity's effective reach statistic.
    /// </returns>
    public static int Reach(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Reach).Current;

    /// <summary>
    /// Gets the entity's effective reach distance in metres.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the reach distance.
    /// </param>
    /// <returns>
    /// The entity's effective reach distance in metres.
    /// </returns>
    public static float ReachMetres(this IEntity entity) =>
        GetMetresFromReach(entity.Reach());
}