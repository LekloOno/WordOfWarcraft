using Godot;
using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats;

[GlobalClass, Tool]
public partial class StatisticResInt : StatisticRes<int, BucketedIntStat>
{
    [Export]
    private int _layers = 2;

    [Export]
    public int Base
    {
        get => GetBase;
        set => SetBase(value);
    }

    public StatisticResInt() : base() {}
    public StatisticResInt(int @base) : base(@base) {}

    protected override BucketedIntStat BuildStat(int @base) => new(@base, _layers);
}