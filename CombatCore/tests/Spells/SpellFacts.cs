using CombatCore;
using CombatCore.Primitives;
using CombatCore.Sources;
using CombatCore.Sources.Attachment;
using CombatCore.Sources.Lifecycle;

namespace ReferenceSpells;

// Facts of the spell layer. The core knows none of this: they are published with Authority.Announce.

public enum StopReason { Completed, Cancelled, Interrupted, RuleViolation }

public sealed record InstanceStarted(Source Instance, uint AbilityId, EntityId Caster, Source? Parent);

/// <summary>The FLOW ended (the node is now pending: its consequences may still be running). SourceTerminated says everything is over.</summary>
public sealed record InstanceStopped(Source Instance, uint AbilityId, EntityId Caster, StopReason Reason);
