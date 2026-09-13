using System;
using WowGd.Src.Combat.Resources;

namespace WowGd.Src.Combat.Health;

public interface IEntityHealth : IStandardResource
{
    event Action?       Died;
    event Action<int>?  Resurrected;

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