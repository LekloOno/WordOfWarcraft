using System.Collections.Immutable;
using CombatCore.Primitives;
using CombatCore.Sources;
using CombatCore.Sources.Lifecycle;

namespace CombatCore;

// ---------- drafts: what an emitter asks for (the authority stamps id, origin, lineage, depth) ----------
public abstract record Draft(Source Source, EntityId Target, Source? Cause);

public sealed record ResourceDraft(
    Source Source, EntityId Target, ResourceId Resource, ResourceDirection Direction, long Amount,
    RequestTagSet Tags = default, Source? Cause = null) : Draft(Source, Target, Cause);

public sealed record StatusDraft(
    Source Source, EntityId Target, StatusOp Op, StatusDefinition? Def = null,
    long Level = 1, long? Duration = null, MergeOp? LevelOp = null, MergeOp? TimeOp = null,
    StatusFilter RemoveFilter = default, RemoveReason Reason = RemoveReason.Dispelled,
    Source? Cause = null) : Draft(Source, Target, Cause);

/// <summary>
/// A lifecycle command as a draft: Insert / Terminate / Kill / Abort / CompleteTermination go through the pipeline like everything else,
/// so they can be intercepted, reacted to and observed. Source = who asks. Insert: Node = parent, Other = child.
/// (Target is stamped by the authority: the entity that owns Node.)
/// </summary>
public sealed record SourceDraft(
    Source Source, SourceCommandKind Command, Source Node, Source? Other = null,
    int Generation = 0, object? Ctx = null, Source? Cause = null) : Draft(Source, default, Cause);

// ---------- requests ----------
public abstract record Request(
    RequestId Id, Source Source, Source? Cause, Origin Origin,
    ImmutableArray<long> Lineage, int Depth, EntityId Target)
{
    public abstract Request CloneValues();
    public abstract int ContributionCount { get; }
    public abstract string Describe();
    public Request WithLineage(ImmutableArray<long> lineage) => this with { Lineage = lineage };

    protected string Tail() =>
        $"[{Origin}{(Cause is { } c ? $", cause {c}" : "")}{(Lineage.Length > 0 ? $", lineage {string.Join(",", Lineage)}" : "")}]";
}

public sealed record ResourceRequest(
    RequestId Id, Source Source, Source? Cause, Origin Origin,
    ImmutableArray<long> Lineage, int Depth, EntityId Target,
    ResourceId Resource, ResourceDirection Direction, RequestTagSet Tags, BucketedValue Value)
    : Request(Id, Source, Cause, Origin, Lineage, Depth, Target)
{
    public override Request CloneValues() => this with { Value = Value.Clone() };
    public override int ContributionCount => Value.Contributions.Count;
    public override string Describe() =>
        $"{Id} {Source} -> {Target} {Direction} res{Resource.Value} {Value.Base} {Tail()}";
}

public sealed record SourceRequest(
    RequestId Id, Source Source, Source? Cause, Origin Origin,
    ImmutableArray<long> Lineage, int Depth, EntityId Target,
    SourceCommandKind Command, Source Node, Source? Other, int Generation, object? Ctx)
    : Request(Id, Source, Cause, Origin, Lineage, Depth, Target)
{
    public override Request CloneValues() => this;      // nothing numeric to protect from modifiers
    public override int ContributionCount => 0;
    public override string Describe() =>
        $"{Id} {Source} -> {Command} {Node}{(Other is { } o ? $" ({o})" : "")} {Tail()}";
}

public sealed record StatusRequest(
    RequestId Id, Source Source, Source? Cause, Origin Origin,
    ImmutableArray<long> Lineage, int Depth, EntityId Target,
    StatusOp Op, StatusDefinition? Def, BucketedValue Level, BucketedValue Duration, bool HasDuration,
    MergeOp? LevelOp, MergeOp? TimeOp, StatusFilter RemoveFilter, RemoveReason Reason)
    : Request(Id, Source, Cause, Origin, Lineage, Depth, Target)
{
    public override Request CloneValues() => this with { Level = Level.Clone(), Duration = Duration.Clone() };
    public override int ContributionCount => Level.Contributions.Count + Duration.Contributions.Count;
    public override string Describe() =>
        $"{Id} {Source} -> {Target} Status.{Op} {Def?.Name ?? "(filter)"} level {Level.Base} {Tail()}";
}

// ---------- facts: final and self-contained ----------
public readonly record struct FactCtx(RequestId Request, Source? Cause, ImmutableArray<long> Lineage, int Depth);

public sealed record ResourceFact(
    FactCtx Ctx, Source Source, EntityId Target, ResourceId Resource, ResourceDirection Direction, RequestTagSet Tags,
    long Requested, long Applied, long Before, long After, IReadOnlyList<Contribution> Contributions);

public sealed record DeathFact(FactCtx Ctx, EntityId Entity, Source Killer);
public sealed record RequestDropped(FactCtx Ctx, string Reason);

public sealed record StatusApplied(FactCtx Ctx, EntityId Owner, uint DefId, uint InstanceId, int Level, long? ExpiresAt, Source Applier);
public sealed record StatusChanged(FactCtx Ctx, EntityId Owner, uint DefId, uint InstanceId, int LevelBefore, int LevelAfter, long? ExpiresBefore, long? ExpiresAfter);
public sealed record StatusRemoved(FactCtx Ctx, EntityId Owner, uint DefId, uint InstanceId, RemoveReason Reason);

// Source tree lifecycle: published by the authority from the lifecycle processor. Cause = the node concerned.
public sealed record SourceInserted(Source Parent, Source Node);
public sealed record SourceTerminating(Source Node);                        // entered PendingTermination (also when born pending)
public sealed record SourceTerminated(Source Node, bool Killed);            // left the tree for good (the node can be inserted again)
public sealed record SourceAborted(Source Node);
public sealed record SourceCommandRejected(SourceCommandKind Command, Source Node, SourceRejectReason Reason);
