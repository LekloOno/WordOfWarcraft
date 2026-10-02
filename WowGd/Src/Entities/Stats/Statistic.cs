using System;
using System.Collections.Generic;

namespace WowGd.Src.Entities.Stats;

public sealed class Statistic<T>(T @base) : IStatistic<T>
{
    private readonly Dictionary<long, IModifier<T>> _modifiers = [];
    private long _nextId;

    public T Base { get; } = @base;
    public T Current { get; private set; } = @base;

    public IDisposable? AddModifier(IModifier<T> modifier)
    {
        var id = _nextId++;

        _modifiers.Add(id, modifier);
        Current = modifier.Apply(Current);

        return new ModifierHandle(this, id);
    }

    private void RemoveModifier(long id)
    {
        if (_modifiers.Remove(id))
            Recalculate();
    }

    private void Recalculate()
    {
        Current = Base;
        foreach (IModifier<T> modifier in _modifiers.Values)
            Current = modifier.Apply(Current);
    }

    private sealed class ModifierHandle(Statistic<T> statistic, long id) : IDisposable
    {
        private Statistic<T>? _statistic = statistic;
        private readonly long _id = id;

        public void Dispose()
        {
            _statistic?.RemoveModifier(_id);
            _statistic = null;
        }
    }
}