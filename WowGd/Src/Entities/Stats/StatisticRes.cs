using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;

namespace WowGd.Src.Entities.Stats;

[Tool]
public abstract partial class StatisticRes<TVal, TStat> : Resource
    where TVal: INumber<TVal>
    where TStat: IStatistic<TVal>
{
    public StatisticRes() { }
    public StatisticRes(TVal @base) : this() { _base = @base; }  

    protected TVal _base = default!;
    public readonly List<BoundStatistic> _boundStats = [];

    protected TVal GetBase => _base;
    protected void SetBase(TVal value)
    {
        if (EqualityComparer<TVal>.Default.Equals(_base, value))
            return;

        _base = value;
        foreach (BoundStatistic stat in _boundStats)
            stat.Stat.Base = value;
    }

    public class BoundStatistic(StatisticRes<TVal, TStat> res, TStat stat) : IDisposable
    {
        public readonly TStat Stat = stat;

        public void Dispose()
        {
            res._boundStats.SwapRemove(this);
            GC.SuppressFinalize(this);
        }
    }

    protected abstract TStat BuildStat(TVal @base);

    public BoundStatistic BindStat()
    {
        BoundStatistic stat = new(this, BuildStat(_base));
        _boundStats.Add(stat);
        return stat;
    }
}