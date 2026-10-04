using System;
using Godot;
using WowGd.Src.Entities.Stats.Bucketing;
using WowGd.Src.Entities.Stats.Modifier;

namespace WowGd.Src.Entities.Stats;

[GlobalClass]
public partial class Statistics : Node, IStatistics
{
    [Export] private StatisticResInt _baseTackle    = null!;
    [Export] private StatisticResInt _baseDodge     = null!;
    [Export] private StatisticResInt _baseSpeed     = null!;
    [Export] private StatisticResInt _baseAgility   = null!;
    [Export] private StatisticResInt _baseGrip      = null!;
    [Export] private StatisticResInt _baseVitality  = null!;
    [Export] private StatisticResInt _baseFocus     = null!;
    

    public IBucketedStat<int> GetStatistic(StatEnum stat) =>
        _values[(int)stat].Stat;

    private readonly StatisticResInt.BoundStatistic[] _values =
        new StatisticResInt.BoundStatistic[Enum.GetValues<StatEnum>().Length];

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
        _baseTackle,
        _baseDodge,
        _baseSpeed,
        _baseAgility,
        _baseGrip,
        _baseVitality,
        _baseFocus
    ];
}