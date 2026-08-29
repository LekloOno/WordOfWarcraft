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

    /// <summary>
    /// Tries to damage the entity.
    /// </summary>
    /// <param name="hp">Damage to inflict. Must be positive.</param>
    /// <returns>Whether the target got killed.</returns>
    bool Damage(int hp);
    /// <summary>
    /// Tries to heal the entity.
    /// </summary>
    /// <param name="hp">Heal to apply. Must be positive.</param>
    /// <returns>Whether the target got healed.</returns>
    bool Heal(int hp);
    /// <summary>
    /// Tries to resurrect the entity. 
    /// </summary>
    /// <param name="hp">
    /// The initial hps to resurrect the target to.
    /// It defaults to `MaxHps`.
    /// </param>
    /// <returns>Whether the target got resurrected.</returns>
    bool Resurrect(int? hp = null);
    /// <summary>
    /// Returns whether the entity is dead, that is, its current hps are below or equal to 0.
    /// </summary>
    /// <returns>Whether the target is dead.</returns>
    bool Dead();
}