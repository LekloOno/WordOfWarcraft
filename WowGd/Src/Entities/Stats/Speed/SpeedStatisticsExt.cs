using WowGd.Src.Entities.Stats.Modifier;

namespace WowGd.Src.Entities.Stats.Speed;

public static class SpeedStatisticsExt
{
    public const float SpeedToMsFactor = 0.1f;

    public static float GetMsFromSpeed(int speed) => speed * SpeedToMsFactor;

    public static float GetMaxSpeedFrom(IEntity entity)
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