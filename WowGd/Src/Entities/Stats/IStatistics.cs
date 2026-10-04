using WowGd.Src.Entities.Stats.Bucketing;
using WowGd.Src.Entities.Stats.Modifier;

namespace WowGd.Src.Entities.Stats;

public interface IStatistics
{
    IBucketedStat<int> GetStatistic(TargetStat targetStat);
}