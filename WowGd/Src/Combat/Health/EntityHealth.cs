using System;
using Godot;

namespace WowGd.Src.Combat.Health;

[GlobalClass]
public partial class EntityHealth : Node, IEntityHealth
{
    [Export] public int Base { get; private set; } = 50;
    public int Max => Base;

    public event Action<int>?   Consumed;
    public event Action<int>?   Generated;
    public event Action?        Died;
    public event Action<int>?   Resurrected;

    public int Current { get; private set; }

    public bool Resurrect(int? hp = null)
    {
        bool dead = Dead();
        if (!dead)
            return false;

        if (hp == 0)
            return false;

        hp ??= Max;
        Current = (int)hp;

        Resurrected?.Invoke(Current);

        return true;
    }

    public bool Consume(int hp, out int overflow)
    {
        overflow = hp;

        if (Dead())
            return true;

        if (hp <= 0)
            return false;

        int consumed = Math.Min(hp, Current);
        overflow = hp - consumed;
        Current -= consumed;

        Consumed?.Invoke(hp);

        bool dead = Dead();
        if (Dead())
            Died?.Invoke();

        return !dead;
    }

    public bool Generate(int hp, out int overflow)
    {
        overflow = hp;
        if (Dead())
            return false;

        if (hp <= 0)
            return false;

        int generated = Math.Min(hp, Max - Current);
        overflow = hp - generated;
        Current += generated;

        Generated?.Invoke(hp);
        
        return true;
    }

    public bool Dead() => Current <= 0;
}