using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats.Modifier;

public enum TargetStat : int
{
    Tackle,
    Dodge,
    Speed,
}

public static class TargetStatExt
{
    public static IBucketedStat<int> GetStat(this TargetStat target, IEntity entity) =>
        entity.Statistics.GetStatistic(target);
}