using System;

namespace WowGd.Src.Entities.Stats;

public interface IStatistic<T>
{
    T Base { get; }
    T Current { get; }
    IDisposable? AddModifier(IModifier<T> modifier);
}