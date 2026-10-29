using CombatCore;
using CombatCore.Primitives;
using CombatCore.Sources;
using CombatCore.Sources.Attachment;
using CombatCore.Sources.Lifecycle;

namespace ReferenceSpells;

/// <summary>The node of an ability of one caster. Runs of the spell are its children.</summary>
public sealed class AbilityAttachment(uint abilityId, EntityId caster) : ISourceAttachment, IAbilityAttachment
{
    public uint AbilityId { get; } = abilityId;
    public EntityId Caster { get; } = caster;
}

/// <summary>
/// One run of a spell's flow, as the ATTACHMENT of its own node: its lifecycle is the node's lifecycle. Stop() asks the node to terminate;
/// the node stays pending while its consequences (child nodes: launches, projectiles...) are alive and is gone when they are done.
/// Everything emitted through it is stamped Source = Cause = the node. Consequences that must outlive the flow are CHILD nodes
/// of the run: they emit through their own node.
/// Every spell would define its own such attachment, with the data and rules of that spell.
/// </summary>
public sealed class SpellInstance(Authority authority, uint abilityId, EntityId caster, Source? parent)
    : ISourceAttachment, IAbilityInstanceAttachment
{
    public uint AbilityId { get; } = abilityId;
    public EntityId Caster { get; } = caster;
    /// <summary>The run that triggered this one, if any. A causal link, not a tree link.</summary>
    public Source? Parent { get; } = parent;
    /// <summary>The node of this run. Set by the book that creates it.</summary>
    public Source Node { get; internal set; } = null!;
    public StopReason? Reason { get; private set; }
    public bool Stopped => Reason is not null;

    public void Request(EntityId target, ResourceId resource, ResourceDirection direction, long amount, RequestTagSet tags = default) =>
        authority.Submit(new ResourceDraft(Node, target, resource, direction, amount, tags, Node));

    public void ApplyStatus(EntityId target, StatusDefinition def, long level = 1, long? duration = null) =>
        authority.Submit(new StatusDraft(Node, target, StatusOp.Apply, def, level, duration, Cause: Node));

    /// <summary>Ends the flow once, with a reason. Terminate is idempotent: the first reason wins.</summary>
    public void Stop(StopReason reason) => authority.Terminate(Node, reason);

    void ISourceAttachment.InsertedTo(Source parentNode, object? ctx) =>
        authority.Announce(new InstanceStarted(Node, AbilityId, Caster, Parent), Node);

    TerminationResponse ISourceAttachment.TerminationRequested(TerminationTicket ticket, object? ctx)
    {
        // The reason travels as the ctx of the Terminate command. A wave started higher up (the entity is removed) carries none:
        // the run is cancelled from outside.
        Reason = ctx is StopReason r ? r : StopReason.Cancelled;
        authority.Announce(new InstanceStopped(Node, AbilityId, Caster, Reason.Value), Node);

        // The flow itself has nothing left to wait for. What keeps the node alive are its children.
        return TerminationResponse.Done;
    }
}
