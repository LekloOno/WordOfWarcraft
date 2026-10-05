using System;
using System.Numerics;
using WowGd.Src.Entities.Stats.Bucketing.Buckets;

namespace WowGd.Src.Entities.Stats.Bucketing;

public abstract class BucketedStat<TVal> : IBucketedStat<TVal>
    where TVal :
        IAdditionOperators<TVal, TVal, TVal>,
        ISubtractionOperators<TVal, TVal, TVal>
{
    public StatBuckets<TVal> Buckets    { get; }

    private TVal _base;
    public abstract TVal Current        { get; }
    public event Action<TVal>? Changed;

    public TVal Base
    {
        get => _base;
        set
        {
            _base = value;
            NotifyChanged();
        }
    }

    private void NotifyChanged() => Changed?.Invoke(Current);

    public BucketedStat(TVal @base, TVal identity, int layers)
    {
        Buckets = new(layers, identity);
        _base = @base;
        Buckets.Changed += NotifyChanged;
    }
}