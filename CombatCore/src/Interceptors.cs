using CombatCore.Primitives;

namespace CombatCore;

/// <summary>Cheap structural discrimination for resource requests. Derive what is derivable, tag only content.</summary>
public readonly record struct RequestFilter(
    ResourceDirection direction = ResourceDirection.None,
    RequestTagSet RequireAll = default,
    RequestTagSet Exclude = default,
    SourceKind? Kind = null,
    uint? SourceId = null,
    OriginMask Origin = OriginMask.Any,
    EntityId? Target = null,
    EntityId? SourceOwner = null)
{
    public bool Matches(ResourceRequest r) =>
        (direction == ResourceDirection.None || (direction & r.Direction) != 0)
        && r.Tags.ContainsAll(RequireAll)
        && !r.Tags.Intersects(Exclude)
        && (Kind is null || Kind == r.Source.Kind)
        && (SourceId is null || SourceId == r.Source.Id)
        && (Origin == OriginMask.Any || (int)Origin == (r.Origin == Primitives.Origin.Original ? 1 : 2))
        && (Target is null || Target == r.Target)
        && (SourceOwner is null || SourceOwner == r.Source.Owner);
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
    public static Modifier<ResourceRequest> Mod(RequestFilter f, Func<ResourceRequest, ValueMod> fn) => new(f.Matches, (r, by) => r.Value.Apply(by, fn(r)));
    public static Modifier<ResourceRequest> Mod(RequestFilter f, ValueMod m) => Mod(f, _ => m);
    public static Modifier<StatusRequest> LevelMod(StatusRequestFilter f, ValueMod m) => new(f.Matches, (r, by) => r.Level.Apply(by, m));
    public static Modifier<StatusRequest> DurationMod(StatusRequestFilter f, ValueMod m) => new(f.Matches, (r, by) => r.Duration.Apply(by, m));
}
