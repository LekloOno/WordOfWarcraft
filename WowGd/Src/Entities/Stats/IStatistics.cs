namespace WowGd.Src.Entities.Stats;

public interface IStatistics
{
    IStatistic<int> Dodge   { get; }
    IStatistic<int> Tackle  { get; }
}