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

public record StatusDefinition(
    string Name, StatusCategory Category,
    GrantTagSet Grants = default,
    LevelModel? Level = null,
    TimeModel? Time = null,
    IReadOnlyList<IStatusEffect>? Effects = null,
    Func<StatusInstance, Authority, IEnumerable<IDisposable>>? Install = null)
{
    private static uint _nextId;

    /// <summary>
    /// Unique per definition object, assigned at construction: nothing to pick by hand, so no collision.
    /// A copy made with `with` keeps its Id (same identity, so it merges with the original on an entity).
    /// Order of construction decides it, so it is only stable inside one process.
    /// </summary>
    public uint Id { get; } = Interlocked.Increment(ref _nextId);

    /// <summary>Builds the typed payload of an instance. Called once, when the authority creates the instance. Plain statuses have none.</summary>
    public virtual object? CreateData(StatusInstance instance, ICombatEntity owner) => null;

    /// <summary>Called when an Apply reaches an already existing instance, before the merge ops. Lets typed data refresh itself.</summary>
    public virtual void OnReapply(StatusInstance instance) { }
}

/// <summary>
/// A status that carries typed data (the mage's breaches...). It IS a status definition: it goes through the usual pipeline,
/// so level, time, effects, expiry, cleanse, facts and owner removal all work. The definition doubles as the typed key
/// to read the data back: <c>authority.TryGetData(owner, BreachStatus.Def, out var breaches)</c>.
/// If the data is IDisposable, the authority disposes it when the instance is removed, whatever the reason.
/// </summary>
public sealed record TypedStatusDefinition<T>(
    string Name, StatusCategory Category,
    Func<StatusInstance, ICombatEntity, T> Create,
    GrantTagSet Grants = default,
    LevelModel? Level = null,
    TimeModel? Time = null,
    IReadOnlyList<IStatusEffect>? Effects = null,
    Func<StatusInstance, Authority, IEnumerable<IDisposable>>? Install = null,
    Action<StatusInstance, T>? Reapply = null)
    : StatusDefinition(Name, Category, Grants, Level, Time, Effects, Install) where T : class
{
    public override object? CreateData(StatusInstance instance, ICombatEntity owner) => Create(instance, owner);

    public override void OnReapply(StatusInstance instance)
    {
        if (Reapply is not null && instance.Data is T data) Reapply(instance, data);
    }
}

/// <summary>Per-target instance of a definition. Owns every handle it registered: disposing them is "the status ends".</summary>
public sealed class StatusInstance(uint id, StatusDefinition def, EntityId owner, int level, long? expiresAt, Source applier)
{
    public uint Id { get; } = id;
    public StatusDefinition Def { get; } = def;
    public EntityId Owner { get; } = owner;
    public int Level { get; internal set; } = level;
    public long? ExpiresAt { get; internal set; } = expiresAt;
    public Source Applier { get; internal set; } = applier;   // latest applier, kept for attribution
    /// <summary>Typed payload (see TypedStatusDefinition). Owned by the instance: disposed with it.</summary>
    public object? Data { get; internal set; }
    internal bool Removed;
    internal readonly List<IDisposable> Handles = [];
    internal readonly List<IEffectHandle> EffectHandles = [];

    /// <summary>What interceptors and stat modifiers scale with. Read live, so level changes need no re-registration.</summary>
    public float Weight => Def.Level is null ? 1f : Def.Level.Curve?.Invoke(Level) ?? Level;
}

/// <summary>Filters statuses by category / tags / definition. Categories == None means "any".</summary>
public readonly record struct StatusFilter(StatusCategory Categories = StatusCategory.None, GrantTagSet RequireAll = default, uint? DefId = null)
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
