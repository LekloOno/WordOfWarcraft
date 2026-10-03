using Godot;

namespace WowGd.Src.Entities.Stats.Bucketing;

public sealed class BucketedIntStat(int @base, int layers) : BucketedStat<int>(@base, 0, layers)
{
    public override int Current => Mathf.FloorToInt(Buckets.Multiplier * Base) + Buckets.Flat;
}