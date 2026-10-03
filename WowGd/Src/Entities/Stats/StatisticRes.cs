using System;
using System.Collections.Generic;
using Godot;

namespace WowGd.Src.Entities.Stats;

[Tool]
public abstract partial class StatisticRes<T> : Resource
    where T: struct
{
    public StatisticRes() { }
    public StatisticRes(T @base) : this() { _base = @base; }  

    protected T _base;
    public readonly List<BoundStatistic> _boundStats = [];

    protected T GetBase => _base;
    protected void SetBase(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_base, value))
            return;

        _base = value;
        foreach (BoundStatistic stat in _boundStats)
            stat.Base = value;
    }

    public class BoundStatistic(StatisticRes<T> res, IStatistic<T> stat) : IDisposable, IStatistic<T>
    {
        public T Base { get => stat.Base; set => stat.Base = value; }
        public T Current => stat.Current;

        public IDisposable? AddModifier(IModifier<T> modifier) =>
            stat.AddModifier(modifier);

        public void Dispose()
        {
            res._boundStats.SwapRemove(this);
            GC.SuppressFinalize(this);
        }
    }

    public BoundStatistic BindStat()
    {
        BoundStatistic stat = new(this, new Statistic<T>(_base));
        _boundStats.Add(stat);
        return stat;
    }
}