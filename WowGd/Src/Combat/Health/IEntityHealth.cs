using System;

namespace WowGd.Src.Combat.Health;

public interface IEntityHealth
{
    int HitPoints   {get;}
    int Current     {get;}

    event Action<int>?  Damaged;
    event Action<int>?  Healed;
    event Action?       Died;
    event Action<int>?  Resurrected;

    bool Damage(int hp);
    bool Heal(int hp);
    bool Dead();
    void Resurrect(int? hp = null);
}