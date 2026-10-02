using Godot;

namespace WowGd.Src.Entities.Stats;

[GlobalClass]
public partial class Statistics : Node, IStatistics
{
    [Export] private int _baseDodge;
    [Export] private int _baseTackle;

    public IStatistic<int> Dodge => new Statistic<int>(_baseDodge);
    public IStatistic<int> Tackle => new Statistic<int>(_baseTackle);
}