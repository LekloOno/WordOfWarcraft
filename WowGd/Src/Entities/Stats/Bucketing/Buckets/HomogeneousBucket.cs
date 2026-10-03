using System.Numerics;

namespace WowGd.Src.Entities.Stats.Bucketing.Buckets;

public sealed partial class HomogeneousBucket<T>(T identity) : StatBucket<T, T>((a, b) => a + b, (a, b) => a - b, identity)
    where T :
        IAdditionOperators<T, T, T>,
        ISubtractionOperators<T, T, T>;