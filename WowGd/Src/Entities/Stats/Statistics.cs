using System;
using Godot;
using WowGd.Src.Entities.Stats.Bucketing;
using WowGd.Src.Entities.Stats.Modifier;

namespace WowGd.Src.Entities.Stats;

[GlobalClass]
public partial class Statistics : Node, IStatistics
{
    [Export] private StatisticResInt _baseTackle    = new(0);
    [Export] private StatisticResInt _baseDodge     = new(0);
    [Export] private StatisticResInt _baseSpeed     = new(30);

    public IBucketedStat<int> GetStatistic(TargetStat stat) =>
        _values[(int)stat].Stat;

    private readonly StatisticResInt.BoundStatistic[] _values =
        new StatisticResInt.BoundStatistic[Enum.GetValues<TargetStat>().Length];

    public override void _Ready()
    {
        // Just to be sure, godot life cycle could be surprising ..
        DisposeStats();

        StatisticResInt[] res = BuildStatisticRes();

        for (int i = 0; i < _values.Length; i++)
            _values[i] = res[i].BindStat();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            DisposeStats();

        base.Dispose(disposing);
    }

    private void DisposeStats()
    {
        for (int i = 0; i < _values.Length; i++)
            _values[i]?.Dispose();
    }

    private StatisticResInt[] BuildStatisticRes() =>
    [
        _baseDodge,
        _baseTackle,
        _baseSpeed,
    ];
}