using System;

namespace WowGd.Src.Combat.Health;

public interface IEntityHealth
{
    int MaxHps   {get;}
    int CurrentHps     {get;}

    event Action<int>?  Damaged;
    event Action<int>?  Healed;
    event Action?       Died;
    event Action<int>?  Resurrected;

    bool Damage(int hp);
    bool Heal(int hp);
    bool Dead();
    bool Resurrect(int? hp = null);
}