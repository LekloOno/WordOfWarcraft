using System;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Stats.Vitality;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Health;

[GlobalClass]
public partial class EntityHealth : Node, IEntityHealth
{
    private IEntity _entity = null!;
    public int Base => _entity.VitalityBase();
    public int Max => _entity.Vitality();

    public event Action<int>?   Consumed;
    public event Action<int>?   Generated;
    public event Action?        Died;
    public event Action<int>?   Resurrected;

    public int Current { get; private set; }

    public override void _Ready()
    {
        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
    }

    public bool Resurrect(int? hp = null)
    {
        bool dead = Dead();
        if (!dead)
            return false;

        if (hp == 0)
            return false;

        hp ??= Max;
        if (hp is int target)
            Current = Mathf.Min(target, Max);
        else
            Current = Max;

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