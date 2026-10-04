namespace WowGd.Src.Entities.Stats.Speed;

/// <summary>
/// Provides utilities for converting and calculating entity speed statistics.
/// </summary>
public static class SpeedStatisticsExt
{
    /// <summary>
    /// Metres per second represented by one speed statistic point.
    /// </summary>
    public const float SpeedToMsFactor = 0.1f;

    /// <summary>
    /// Gets the entity's effective speed statistic.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to retrieve the speed statistic.
    /// </param>
    /// <returns>
    /// The entity's effective speed statistic.
    /// </returns>
    public static int Speed(this IEntity entity) =>
        entity.Statistics.GetStatistic(StatEnum.Speed).Current;

    /// <summary>
    /// Converts the given speed statistic into a speed value expressed in
    /// metres per second.
    /// </summary>
    /// <param name="speed">
    /// The speed statistic value to convert.
    /// </param>
    /// <returns>
    /// The equivalent speed in metres per second.
    /// </returns>
    public static float GetMsFromSpeed(int speed) => speed * SpeedToMsFactor;

    /// <summary>
    /// Gets the entity's effective maximum movement speed in metres per second,
    /// taking the entity's current speed statistic and tackle speed limit into account.
    /// </summary>
    /// 
    /// <remarks>
    /// If the current speed is below the tackle speed limit, the current speed
    /// is returned unchanged.
    ///
    /// If the current speed is above the base speed, the tackle speed limit is
    /// treated as a strict maximum.
    ///
    /// If the current speed is below the base speed, the tackle speed limit is
    /// scaled proportionally to the current-to-base speed ratio.
    /// </remarks>
    /// 
    /// <param name="entity">
    /// The entity whose effective maximum speed is calculated.
    /// </param>
    /// <returns>
    /// The entity's effective maximum speed in metres per second.
    /// </returns>
    public static float SpeedMs(this IEntity entity)
    {
        IStatistic<int> speedStat = StatEnum.Speed.GetStat(entity);

        float speed = GetMsFromSpeed(speedStat.Current);
        float tackleSpeed = entity.EntityMover.DynamicTackleNode.EffectiveLimit;

        if (tackleSpeed >= speed)
            return speed;
        
        float baseSpeed = GetMsFromSpeed(speedStat.Base);

        // Tackle speed limit is strict.
        // If the player speed is buffed, and goes beyond tackle speed,
        // it is still limited to tackle speed.
        if (speed >= baseSpeed)
            return tackleSpeed;

        // But if the player speed is debuffed, we want to take
        // that debuff into account for the tackle speed too.
        float baseRatio = speed/baseSpeed;

        return tackleSpeed * baseRatio;
    }
}