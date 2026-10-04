using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats;

public interface IStatistics
{
    IBucketedStat<int> GetStatistic(StatEnum targetStat);
}