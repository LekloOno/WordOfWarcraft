using System;

namespace WowGd.Src.Combat.Resources;

public interface IStandardResource
{
    int Base    { get; }
    int Max     { get; }
    int Current { get; }

    event Action<int>? Consumed;
    event Action<int>? Generated;
    event Action<int>? MaxChanged;

    bool Consume(int rp, out int overflow);
    bool Generate(int rp, out int overflow);
}