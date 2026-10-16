using CombatCore.Primitives;

namespace CombatCore;

/// <summary>Cheap structural discrimination for resource requests. Derive what is derivable, tag only content.</summary>
public readonly record struct RequestFilter(
    Role Roles = Role.None,
    TagSet RequireAll = default,
    TagSet Exclude = default,
    SourceKind? Kind = null,
    uint? SourceId = null,
    OriginMask Origin = OriginMask.Any,
    EntityId? Target = null,
    EntityId? SourceOwner = null)
{
    public bool Matches(ResourceRequest r) =>
        (Roles == Role.None || (Roles & r.Role) != 0)
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
public sealed record Replacement<T>(uint Id, Func<T, bool> Match, Phase Phase, Func<T, T> Apply) where T : Request;
public sealed record Addition<T>(uint Id, Func<T, bool> Match, Func<T, IEnumerable<Draft>> Add) where T : Request;
public sealed record Modifier<T>(uint Id, Func<T, bool> Match, Action<T, uint> Apply) where T : Request;

/// <summary>Small helpers so common rules read well.</summary>
public static class Rules
{
    public static Replacement<ResourceRequest> Replace(uint id, RequestFilter f, Phase p, Func<ResourceRequest, ResourceRequest> fn) => new(id, f.Matches, p, fn);
    public static Addition<ResourceRequest> AddOn(uint id, RequestFilter f, Func<ResourceRequest, IEnumerable<Draft>> fn) => new(id, f.Matches, fn);
    public static Modifier<ResourceRequest> Mod(uint id, RequestFilter f, Func<ResourceRequest, ValueMod> fn) => new(id, f.Matches, (r, by) => r.Value.Apply(by, fn(r)));
    public static Modifier<ResourceRequest> Mod(uint id, RequestFilter f, ValueMod m) => Mod(id, f, _ => m);
    public static Modifier<StatusRequest> LevelMod(uint id, StatusRequestFilter f, ValueMod m) => new(id, f.Matches, (r, by) => r.Level.Apply(by, m));
    public static Modifier<StatusRequest> DurationMod(uint id, StatusRequestFilter f, ValueMod m) => new(id, f.Matches, (r, by) => r.Duration.Apply(by, m));
}
