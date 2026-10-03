using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats.Modifier;

public enum TargetStat
{
    Tackle,
    Dodge,
    Speed,
}

public static class TargetStatExt
{
    public static IBucketedStat<int> GetStat(this TargetStat target, IEntity entity)
    {
        return target switch
        {
            TargetStat.Tackle => entity.Statistics.Tackle,
            TargetStat.Dodge => entity.Statistics.Tackle,
            TargetStat.Speed => entity.Statistics.Tackle,
            _ => throw new System.IndexOutOfRangeException(),
        };
    }
}