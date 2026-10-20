using System;
using CombatCore;
using WowGd.Src.Entities.Stats.Bucketing;
using WowGd.Src.Entities.Stats.Bucketing.Buckets;

namespace WowGd.Src.Entities.Stats;

public class StatisticSink : IStatistics
{
    public readonly static StatisticSink Sink = new();
    private readonly static SinkHandle _sinkHandle = new();
    private readonly static SinkStat _sinkStat = new();

    public IEffectHandle Apply(StatContribution contribution, float weight) => _sinkHandle;
    public IBucketedStat<int> GetStatistic(StatEnum targetStat) => _sinkStat;

    private class SinkHandle : IEffectHandle
    {
        public void Dispose() { }
        public void Reweight(float weight) { }
    }

    private class SinkStat : IBucketedStat<int>
    {
        public StatBuckets<int> Buckets => new(1, 0);
        public int Base { get; set; } = 0;
        public int Current => 0;
        public event Action<int>? Changed;
    }
}