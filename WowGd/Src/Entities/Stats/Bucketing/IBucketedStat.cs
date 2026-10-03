using System.Numerics;
using WowGd.Src.Entities.Stats.Bucketing.Buckets;

namespace WowGd.Src.Entities.Stats.Bucketing;

public interface IBucketedStat<TVal> : IStatistic<TVal>
    where TVal :
        IAdditionOperators<TVal, TVal, TVal>,
        ISubtractionOperators<TVal, TVal, TVal>
{
    StatBuckets<TVal> Buckets { get; }
}