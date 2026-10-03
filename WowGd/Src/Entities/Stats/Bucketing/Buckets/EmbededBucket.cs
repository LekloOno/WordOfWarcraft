using System.Numerics;

namespace WowGd.Src.Entities.Stats.Bucketing.Buckets;

public sealed partial class EmbededBucket<TIn, TOut>(TOut identity)
    : StatBucket<StatBucket<TIn, TOut>, TOut>((a, b) => a + b.Modifier, (a, c) => a - c.Modifier, identity)
    where TOut :
        IAdditionOperators<TOut, TOut, TOut>,
        ISubtractionOperators<TOut, TOut, TOut>;