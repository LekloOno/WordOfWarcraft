using CombatCore;
using CombatCore.Primitives;
using CombatCore.Sources;
using CombatCore.Sources.Attachment;
using CombatCore.Sources.Lifecycle;

namespace ReferenceSpells;

// Capabilities a node can have, found by climbing the tree (see FindAttachment<T> and SourceFilter.Having<T>).
// Each one is a question interceptors and handlers can ask about a source.

/// <summary>This node is an ability of a caster.</summary>
public interface IAbilityAttachment
{
    uint AbilityId { get; }
    EntityId Caster { get; }
}

/// <summary>This node is one run of an ability.</summary>
public interface IAbilityInstanceAttachment
{
    /// <summary>Null while the flow runs.</summary>
    StopReason? Reason { get; }
    bool Stopped => Reason is not null;
}

public static class SpellTags
{
    public static readonly SourceTag Spell = SourceTag.Register("Spell");        // an ability of an entity
    public static readonly SourceTag Instance = SourceTag.Register("Instance");  // one run of a spell
}
