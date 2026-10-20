using System.Collections.Immutable;
using CombatCore.Primitives;

namespace CombatCore;

// ---------- drafts: what an emitter asks for (the authority stamps id, origin, lineage, depth) ----------
public abstract record Draft(Source Source, EntityId Target, InstanceId? Cause);

public sealed record RequestDraft(
    Source Source, EntityId Target, ResourceId Resource, ResourceDirection Direction, long Amount,
    TagSet Tags = default, InstanceId? Cause = null) : Draft(Source, Target, Cause);

public sealed record TypedStatusDraft<T>(T Instance, StatusDraft? BaseDraft = null);

public sealed record StatusDraft(
    Source Source, EntityId Target, StatusOp Op, StatusDefinition? Def = null,
    long Level = 1, long? Duration = null, MergeOp? LevelOp = null, MergeOp? TimeOp = null,
    StatusFilter RemoveFilter = default, RemoveReason Reason = RemoveReason.Dispelled,
    InstanceId? Cause = null) : Draft(Source, Target, Cause);

// ---------- requests ----------
public abstract record Request(
    RequestId Id, Source Source, InstanceId? Cause, Origin Origin,
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
    RequestId Id, Source Source, InstanceId? Cause, Origin Origin,
    ImmutableArray<long> Lineage, int Depth, EntityId Target,
    ResourceId Resource, ResourceDirection Direction, TagSet Tags, BucketedValue Value)
    : Request(Id, Source, Cause, Origin, Lineage, Depth, Target)
{
    public override Request CloneValues() => this with { Value = Value.Clone() };
    public override int ContributionCount => Value.Contributions.Count;
    public override string Describe() =>
        $"{Id} {Source.Kind}:{Source.Id} -> {Target} {Direction} res{Resource.Value} {Value.Base} {Tail()}";
}

public sealed record StatusRequest(
    RequestId Id, Source Source, InstanceId? Cause, Origin Origin,
    ImmutableArray<long> Lineage, int Depth, EntityId Target,
    StatusOp Op, StatusDefinition? Def, BucketedValue Level, BucketedValue Duration, bool HasDuration,
    MergeOp? LevelOp, MergeOp? TimeOp, StatusFilter RemoveFilter, RemoveReason Reason)
    : Request(Id, Source, Cause, Origin, Lineage, Depth, Target)
{
    public override Request CloneValues() => this with { Level = Level.Clone(), Duration = Duration.Clone() };
    public override int ContributionCount => Level.Contributions.Count + Duration.Contributions.Count;
    public override string Describe() =>
        $"{Id} {Source.Kind}:{Source.Id} -> {Target} Status.{Op} {Def?.Name ?? "(filter)"} level {Level.Base} {Tail()}";
}

// ---------- facts: final and self-contained ----------
public readonly record struct FactCtx(RequestId Request, InstanceId? Cause, ImmutableArray<long> Lineage, int Depth);

public sealed record ResourceFact(
    FactCtx Ctx, Source Source, EntityId Target, ResourceId Resource, ResourceDirection Direction, TagSet Tags,
    long Requested, long Applied, long Before, long After, IReadOnlyList<Contribution> Contributions);

public sealed record DeathFact(FactCtx Ctx, EntityId Entity, Source Killer);
public sealed record RequestDropped(FactCtx Ctx, string Reason);

public sealed record StatusApplied(FactCtx Ctx, EntityId Owner, uint DefId, uint InstanceId, int Level, long? ExpiresAt, Source Applier);
public sealed record StatusChanged(FactCtx Ctx, EntityId Owner, uint DefId, uint InstanceId, int LevelBefore, int LevelAfter, long? ExpiresBefore, long? ExpiresAfter);
public sealed record StatusRemoved(FactCtx Ctx, EntityId Owner, uint DefId, uint InstanceId, RemoveReason Reason);

// Spell lifecycle: plain facts, no request behind them.
public enum StopReason { Completed, Cancelled, Interrupted, RuleViolation }
public sealed record InstanceStarted(InstanceId Instance, uint AbilityId, EntityId Caster, InstanceId? Parent);
public sealed record InstanceStopped(InstanceId Instance, uint AbilityId, EntityId Caster, StopReason Reason);
