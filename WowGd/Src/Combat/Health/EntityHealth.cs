using System;
using Godot;

namespace WowGd.Src.Combat.Health;

[GlobalClass]
public partial class EntityHealth : Node, IEntityHealth
{
    [Export] public int HitPoints { get; private set; } = 50;

    public event Action<int>?   Damaged;
    public event Action<int>?   Healed;
    public event Action?        Died;
    public event Action<int>?   Resurrected;

    public int Current { get; private set; }

    public bool Resurrect(int? hp = null)
    {
        bool dead = Dead();
        if (!dead)
            return false;

        hp ??= HitPoints;
        Current = (int)hp;

        Resurrected?.Invoke(Current);
        return true;
    }

    public bool Damage(int hp)
    {
        Current -= hp;
        Damaged?.Invoke(hp);

        bool dead = Dead();
        if (dead)
            Died?.Invoke();

        return dead;
    }

    public bool Heal(int hp)
    {
        if (Dead())
            return true;

        int max  = HitPoints - Current;
        int heal = Math.Min(max, hp);
        Current += heal;
        Healed?.Invoke(hp);
        return false;
    }

    public bool Dead() => Current <= 0;
}