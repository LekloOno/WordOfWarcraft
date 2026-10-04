using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats;

public enum StatEnum : int
{
    Tackle,
    Dodge,
    Speed,
}

public static class TargetStatExt
{
    public static IBucketedStat<int> GetStat(this StatEnum target, IEntity entity) =>
        entity.Statistics.GetStatistic(target);
}