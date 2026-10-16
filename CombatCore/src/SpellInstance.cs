using CombatCore.Primitives;

namespace CombatCore;

/// <summary>
/// One run of a spell's flow. The spell asks the authority for it, then emits every request through it,
/// so Source (which ability, whose) and Cause (which run) are stamped for free and can't be forgotten.
/// A stopped instance may still emit: consequences (a projectile in flight) outlive the flow.
/// </summary>
public sealed class SpellInstance
{
    readonly Authority _authority;

    internal SpellInstance(Authority authority, InstanceId id, uint abilityId, EntityId caster, InstanceId? parent)
    { _authority = authority; Id = id; AbilityId = abilityId; Caster = caster; Parent = parent; }

    public InstanceId Id { get; }
    public uint AbilityId { get; }
    public EntityId Caster { get; }
    public InstanceId? Parent { get; }
    public bool Stopped { get; internal set; }
    public Source Source => new(SourceKind.Ability, AbilityId, Caster);

    public void Request(EntityId target, ResourceId resource, Role role, long amount, TagSet tags = default) =>
        _authority.Submit(new RequestDraft(Source, target, resource, role, amount, tags, Id));

    public void ApplyStatus(EntityId target, StatusDefinition def, long level = 1, long? duration = null) =>
        _authority.Submit(new StatusDraft(Source, target, StatusOp.Apply, def, level, duration, Cause: Id));

    /// <summary>Ends the flow once, with a reason. Further calls are ignored.</summary>
    public void Stop(StopReason reason) => _authority.StopInstance(this, reason);
}
