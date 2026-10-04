using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats;

public interface IStatistics
{
    IBucketedStat<int> Dodge    { get; }
    IBucketedStat<int> Tackle   { get; }
    IBucketedStat<int> Speed    { get; }
}