using CombatCore.Primitives;

namespace CombatCore;

// Everything the core needs from the outside world, as small interfaces.
// The game project implements them; tests implement them with fakes.

/// <summary>Time source. The game backs it with the physics tick; tests with a manual counter.</summary>
public interface IGameClock { long Tick { get; } }

public enum ResourceDirection { Remove, Add }

/// <summary>
/// What actually happened to a resource. The adapter does the clamping, so Before/After are real.
/// Killed = this change made the entity go from alive to dead (the adapter knows what "lethal" means).
/// </summary>
public readonly record struct ResourceChange(long Before, long After, bool Killed)
{
    public long Applied => Math.Abs(After - Before);
}

public interface IResourcePort
{
    bool Has(ResourceId id);
    ResourceChange Apply(ResourceId id, ResourceDirection direction, long amount);
}

/// <summary>Handle to something a status installed. Disposing it undoes it; Reweight rescales it in place.</summary>
public interface IEffectHandle : IDisposable { void Reweight(float weight); }

public interface IStatPort
{
    IEffectHandle Apply(StatContribution contribution, float weight);
}

/// <summary>The one adapter per entity: how the core sees an entity of the game.</summary>
public interface ICombatEntity
{
    EntityId Id { get; }
    IResourcePort Resources { get; }
    IStatPort Stats { get; }
}

public interface IEntityDirectory { ICombatEntity? Find(EntityId id); }

/// <summary>
/// One contribution of a status (stat modifiers, movement flags, combat flags, later movement contributions...).
/// The core knows nothing about what it does: it applies it when the status appears and disposes the handle when it ends.
/// </summary>
public interface IStatusEffect { IEffectHandle Apply(StatusInstance instance, ICombatEntity owner); }

/// <summary>The stat contribution as a status effect. Implemented in the core because it only needs IStatPort.</summary>
public sealed class StatEffect(StatContribution contribution) : IStatusEffect
{
    public IEffectHandle Apply(StatusInstance instance, ICombatEntity owner) =>
        owner.Stats.Apply(contribution, instance.Weight);
}
