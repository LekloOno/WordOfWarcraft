using CombatCore.Primitives;

namespace CombatCore;

/// <summary>Closed, engine-level: used by cleanses and UI. Defined from the owner's point of view.</summary>
[Flags]
public enum StatusCategory { None = 0, Neutral = 1, Buff = 2, Debuff = 4, Passive = 8 }

public enum StatusOp { Apply, Adjust, Remove }
public enum RemoveReason { Expired, Dispelled, ZeroLevel, Consumed, OwnerRemoved }

/// <summary>One small closed set of operations, used for BOTH the level axis and the time axis.</summary>
public enum MergeOp { Keep, Replace, Add, Subtract, Max }

/// <summary>Level axis. null on the definition = the status has no level. Curve maps level -> weight (default: weight = level).</summary>
public sealed record LevelModel(int Max, MergeOp OnApply, bool EndsAtZero = false, Func<int, float>? Curve = null);

/// <summary>Time axis. null on the definition = untimed. Durations are in clock ticks.</summary>
public sealed record TimeModel(MergeOp OnApply);

public sealed record StatusDefinition(
    uint Id, string Name, StatusCategory Category,
    TagSet Grants = default,
    LevelModel? Level = null,
    TimeModel? Time = null,
    IReadOnlyList<IStatusEffect>? Effects = null,
    Func<StatusInstance, Authority, IEnumerable<IDisposable>>? Install = null);

/// <summary>Per-target instance of a definition. Owns every handle it registered: disposing them is "the status ends".</summary>
public sealed class StatusInstance(uint id, StatusDefinition def, EntityId owner, int level, long? expiresAt, Source applier)
{
    public uint Id { get; } = id;
    public StatusDefinition Def { get; } = def;
    public EntityId Owner { get; } = owner;
    public int Level { get; internal set; } = level;
    public long? ExpiresAt { get; internal set; } = expiresAt;
    public Source Applier { get; internal set; } = applier;   // latest applier, kept for attribution
    internal bool Removed;
    internal readonly List<IDisposable> Handles = [];
    internal readonly List<IEffectHandle> EffectHandles = [];

    /// <summary>What interceptors and stat modifiers scale with. Read live, so level changes need no re-registration.</summary>
    public float Weight => Def.Level is null ? 1f : Def.Level.Curve?.Invoke(Level) ?? Level;
}

/// <summary>Filters statuses by category / tags / definition. Categories == None means "any".</summary>
public readonly record struct StatusFilter(StatusCategory Categories = StatusCategory.None, TagSet RequireAll = default, uint? DefId = null)
{
    public bool Matches(StatusDefinition d) =>
        (Categories == StatusCategory.None || (Categories & d.Category) != 0)
        && d.Grants.ContainsAll(RequireAll)
        && (DefId is null || DefId == d.Id);

    /// <summary>For removals: a catch-all filter never touches passives, they must be asked for explicitly.</summary>
    public bool MatchesForRemoval(StatusDefinition d) =>
        Matches(d) && (Categories != StatusCategory.None || d.Category != StatusCategory.Passive);
}

public readonly record struct StatusRequestFilter(
    StatusFilter Status = default, StatusOp? Op = null, OriginMask Origin = OriginMask.Any, EntityId? Target = null)
{
    public bool Matches(StatusRequest r) =>
        (Op is null || Op == r.Op)
        && (Origin == OriginMask.Any || (int)Origin == (r.Origin == Primitives.Origin.Original ? 1 : 2))
        && (Target is null || Target == r.Target)
        && (Status == default || (r.Def is { } d && Status.Matches(d)));
}
