using System;
using Godot;

namespace WowGd.Src.Combat.Health;

[GlobalClass]
public partial class EntityHealth : Node, IEntityHealth
{
    [Export] public int MaxHps { get; private set; } = 50;

    public event Action<int>?   Damaged;
    public event Action<int>?   Healed;
    public event Action?        Died;
    public event Action<int>?   Resurrected;

    public int CurrentHps { get; private set; }

    public bool Resurrect(int? hp = null)
    {
        bool dead = Dead();
        if (!dead)
            return false;

        if (hp == 0)
            return false;

        hp ??= MaxHps;
        CurrentHps = (int)hp;

        Resurrected?.Invoke(CurrentHps);

        return true;
    }

    public bool Damage(int hp)
    {
        if (Dead())
            return true;

        if (hp <= 0)
            return false;

        CurrentHps -= hp;
        Damaged?.Invoke(hp);

        bool dead = Dead();
        if (dead)
            Died?.Invoke();

        return dead;
    }

    public bool Heal(int hp)
    {
        if (Dead())
            return false;

        if (hp <= 0)
            return false;

        int max  = MaxHps - CurrentHps;
        int heal = Math.Min(max, hp);
        CurrentHps += heal;
        Healed?.Invoke(hp);
        return true;
    }

    public bool Dead() => CurrentHps <= 0;
}