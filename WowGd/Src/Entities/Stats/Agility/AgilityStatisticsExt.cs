namespace WowGd.Src.Entities.Stats.Agility;

/// <summary>
/// Provides utilities for converting and calculating entity agility statistics.
/// </summary>
public static class AgilityStatisticsExt
{
    /// <summary>
    /// m·s⁻² represented by one agility statistic point.
    /// </summary>
    public const float AgilityToAccelerationMs2Factor = 1f;

    /// <summary>
    /// Converts the given agility statistic into an acceleration value
    /// expressed in m·s⁻².
    /// </summary>
    /// <param name="speed">
    /// The agility statistic value to convert.
    /// </param>
    /// <returns>
    /// The equivalent acceleration in m·s⁻².
    /// </returns>
    public static float GetAccelerationMs2FromAgility(int speed) => speed * AgilityToAccelerationMs2Factor;

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

    /// <summary>
    /// Gets the entity's effective acceleration in m·s⁻².
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the acceleration.
    /// </param>
    /// <returns>
    /// The entity's effective acceleration in m·s⁻².
    /// </returns>
    public static float AccelerationMs2(this IEntity entity) =>
        GetAccelerationMs2FromAgility(entity.Agility());
}