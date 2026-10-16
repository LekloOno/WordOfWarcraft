using CombatCore;
using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats;

public interface IStatistics : IStatPort
{
    IBucketedStat<int> GetStatistic(StatEnum targetStat);
}