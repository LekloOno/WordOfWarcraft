using CombatCore.Primitives;
using CombatCore.Sources;
using CombatCore.Sources.Lifecycle;

namespace CombatCore;

/// <summary>
/// Filters ONE node of the source tree (the source of a request, its cause, the node a lifecycle command is about...).
/// Everything unset matches, so default(SourceFilter) is a valid match-all. Cheap checks first, tree climbs, custom code last.
/// Inherited = the node's InheritedTags (its own tags AND its ancestors'): "comes from an entity".
/// Exact = the node's own Tags only: "is itself a status".
/// <see cref="Having{T}"/> / <see cref="HavingExact{T}"/> find the node by what its attachment IS (a capability interface), which is
/// how layers the core knows nothing about (spells, passives...) are filtered on.
/// </summary>
public readonly record struct SourceFilter(
    SourceTagSet RequireAll = default,        // inherited: all of these
    SourceTagSet Exclude = default,           // inherited: none of these
    SourceTagSet ExactRequireAll = default,   // own tags: all of these
    SourceTagSet ExactExclude = default,      // own tags: none of these
    Source? Is = null,                        // exactly this node
    Source? Under = null,                     // this node or any descendant of it
    EntityId? Owner = null,                   // owned by this entity
    Func<Source, bool>? Where = null)         // anything else (built once, when the rule is created)
{
    public bool IsAny => this == default;

    public bool Matches(Source s) =>
        (Is is null || s == Is)
        && s.InheritedTags.ContainsAll(RequireAll) && !s.InheritedTags.Intersects(Exclude)
        && s.Tags.ContainsAll(ExactRequireAll) && !s.Tags.Intersects(ExactExclude)
        && (Under is null || s.IsSameOrDescendantOf(Under))
        && (Owner is null || s.OwnerOf() == Owner)
        && (Where is null || Where(s));

    /// <summary>For an optional node (a cause): no node only matches a filter that asks nothing.</summary>
    public bool MatchesOptional(Source? s) => s is null ? IsAny : Matches(s);

    /// <summary>The node, or the nearest ancestor, has an attachment of type T (that satisfies <paramref name="where"/>).</summary>
    public static SourceFilter Having<T>(Func<T, bool>? where = null) where T : class =>
        new(Where: s => s.FindAttachment<T>() is { } a && (where is null || where(a)));

    /// <summary>The attachment of the node ITSELF is of type T (that satisfies <paramref name="where"/>).</summary>
    public static SourceFilter HavingExact<T>(Func<T, bool>? where = null) where T : class =>
        new(Where: s => s.Attachment is T a && (where is null || where(a)));
}

/// <summary>Cheap structural discrimination for resource requests. Derive what is derivable, tag only content.</summary>
public readonly record struct RequestFilter(
    ResourceDirection direction = ResourceDirection.None,
    RequestTagSet RequireAll = default,
    RequestTagSet Exclude = default,
    SourceFilter Source = default,
    SourceFilter Cause = default,
    OriginMask Origin = OriginMask.Any,
    EntityId? Target = null)
{
    public bool Matches(ResourceRequest r) =>
        (direction == ResourceDirection.None || (direction & r.Direction) != 0)
        && r.Tags.ContainsAll(RequireAll)
        && !r.Tags.Intersects(Exclude)
        && (Origin == OriginMask.Any || (int)Origin == (r.Origin == Primitives.Origin.Original ? 1 : 2))
        && (Target is null || Target == r.Target)
        && Source.Matches(r.Source)
        && Cause.MatchesOptional(r.Cause);
}

/// <summary>Filter for lifecycle requests. Node = the node the command is about (Insert: the parent), Child = Insert only.</summary>
public readonly record struct SourceRequestFilter(
    SourceCommandKind? Command = null,
    SourceFilter Node = default,
    SourceFilter Child = default,
    SourceFilter Source = default,
    SourceFilter Cause = default,
    OriginMask Origin = OriginMask.Any,
    EntityId? Target = null)
{
    public bool Matches(SourceRequest r) =>
        (Command is null || Command == r.Command)
        && (Origin == OriginMask.Any || (int)Origin == (r.Origin == Primitives.Origin.Original ? 1 : 2))
        && (Target is null || Target == r.Target)
        && Node.Matches(r.Node)
        && (Child.IsAny || (r.Other is { } c && Child.Matches(c)))
        && Source.Matches(r.Source)
        && Cause.MatchesOptional(r.Cause);
}

/// <summary>Closed, engine-owned ordering phases (relations, not magic numbers).</summary>
public enum Phase { Convert = 0, Redirect = 1 }

// One set of interceptor shapes, generic over the request type. Match is a plain predicate.
public sealed record Replacement<T>(Func<T, bool> Match, Phase Phase, Func<T, T> Apply) where T : Request;
public sealed record Addition<T>(Func<T, bool> Match, Func<T, IEnumerable<Draft>> Add) where T : Request;
public sealed record Modifier<T>(Func<T, bool> Match, Action<T, long> Apply) where T : Request;

/// <summary>Small helpers so common rules read well.</summary>
public static class Rules
{
    public static Replacement<ResourceRequest> Replace(RequestFilter f, Phase p, Func<ResourceRequest, ResourceRequest> fn) => new(f.Matches, p, fn);
    public static Addition<ResourceRequest> AddOn(RequestFilter f, Func<ResourceRequest, IEnumerable<Draft>> fn) => new(f.Matches, fn);
    // lifecycle requests: nothing numeric to modify, so only replacements and additions
    public static Replacement<SourceRequest> ReplaceSource(SourceRequestFilter f, Phase p, Func<SourceRequest, SourceRequest> fn) => new(f.Matches, p, fn);
    public static Addition<SourceRequest> AddOnSource(SourceRequestFilter f, Func<SourceRequest, IEnumerable<Draft>> fn) => new(f.Matches, fn);
    public static Modifier<ResourceRequest> Mod(RequestFilter f, Func<ResourceRequest, ValueMod> fn) => new(f.Matches, (r, by) => r.Value.Apply(by, fn(r)));
    public static Modifier<ResourceRequest> Mod(RequestFilter f, ValueMod m) => Mod(f, _ => m);
    public static Modifier<StatusRequest> LevelMod(StatusRequestFilter f, ValueMod m) => new(f.Matches, (r, by) => r.Level.Apply(by, m));
    public static Modifier<StatusRequest> DurationMod(StatusRequestFilter f, ValueMod m) => new(f.Matches, (r, by) => r.Duration.Apply(by, m));
}
