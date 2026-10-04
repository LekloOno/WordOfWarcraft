namespace WowGd.Src.Entities.Stats.Reach;

public static class ReachStatisticsExt
{
    public const float ReachToMetersFactor = 0.1f;

    public static float GetMetersFromReach(int reach) => reach * ReachToMetersFactor;
    public static float GetMetersReach(this IEntity entity) =>
        GetMetersFromReach(entity.Statistics.GetStatistic(StatEnum.Reach).Current);
}