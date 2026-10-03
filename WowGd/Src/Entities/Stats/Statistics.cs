using Godot;
using WowGd.Src.Entities.Stats.Bucketing;

namespace WowGd.Src.Entities.Stats;

[GlobalClass]
public partial class Statistics : Node, IStatistics
{
    [Export] private StatisticResInt _baseTackle = new(0);
    [Export] private StatisticResInt _baseDodge  = new(0);
    [Export] private StatisticResInt _baseSpeed  = new(30);

    public IBucketedStat<int> Dodge  => _dodge.Stat;
    public IBucketedStat<int> Tackle => _tackle.Stat;
    public IBucketedStat<int> Speed  => _speed.Stat;

    private StatisticResInt.BoundStatistic _dodge   = null!;
    private StatisticResInt.BoundStatistic _tackle  = null!;
    private StatisticResInt.BoundStatistic _speed  = null!;

    public override void _Ready()
    {
        // Just to be sure, godot life cycle could be surprising ..
        _dodge?.Dispose();
        _tackle?.Dispose();
        _speed?.Dispose();

        _dodge  = _baseDodge.BindStat();
        _tackle = _baseTackle.BindStat();
        _speed  = _baseSpeed.BindStat();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dodge?.Dispose();
            _tackle?.Dispose();
            _speed?.Dispose();
        }

        base.Dispose(disposing);
    }
}