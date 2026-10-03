using System.Numerics;
using WowGd.Src.Entities.Stats.Bucketing.Buckets;

namespace WowGd.Src.Entities.Stats.Bucketing;

public abstract class BucketedStat<TVal>(TVal @base, TVal identity, int layers) : IBucketedStat<TVal>
    where TVal :
        IAdditionOperators<TVal, TVal, TVal>,
        ISubtractionOperators<TVal, TVal, TVal>
{
    public StatBuckets<TVal> Buckets { get; } = new(layers, identity);

    public TVal Base { get; set; } = @base;
    public abstract TVal Current { get; }
}