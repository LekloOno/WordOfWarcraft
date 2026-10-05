using System;

namespace WowGd.Src.Entities.Stats;

public interface IStatistic<T>
{
    T Base { get; set; }
    T Current { get; }
    event Action<T>? Changed;
}