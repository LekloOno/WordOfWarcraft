using System;

namespace WowGd.Src.Combat.Health;

public class EnvironmentHealth : IEntityHealth
{
    public readonly static EnvironmentHealth Instance = new();

    public int Base => 0;
    public int Max => 0;
    public int Current => 0;

    public event Action<int>?   Consumed;
    public event Action<int>?   Generated;
    public event Action?        Died;
    public event Action<int>?   Resurrected;
    public event Action<int>?   MaxChanged;

    public bool Dead() => false;
    public bool Resurrect(int? hp) => false;
    public bool Consume(int hp, out int overflow)
    {
        overflow = hp;
        return false;
    }
    public bool Generate(int hp, out int overflow)
    {
        overflow = hp;
        return false;
    }
}