namespace CombatCore.Primitives;

// Plain data only: everything here must be serializable later (facts are the wire protocol).
public readonly record struct EntityId(uint Value)   { public override string ToString() => $"E{Value}"; }
public readonly record struct InstanceId(uint Value) { public override string ToString() => $"#{Value}"; }
public readonly record struct RequestId(uint Value)  { public override string ToString() => $"r{Value}"; }

/// <summary>Open id (the game side defines the constants). The core never needs to know which one is "health".</summary>
public readonly record struct ResourceId(int Value);

/// <summary>
/// Closed, engine-owned set: the pipeline itself branches on it (clamping, kill detection, ...).
/// A request has exactly one role; a filter may hold a mask of roles (None = any).
/// </summary>
[Flags]
public enum Role { None = 0, Damage = 1, Drain = 2, Cost = 4, Heal = 8, Refund = 16 }

public enum SourceKind { System, Ability, Status }

/// <summary>Who emitted the request. Mandatory, unlike Cause.</summary>
public readonly record struct Source(SourceKind Kind, uint Id, EntityId Owner);

public enum Origin { Original, Derived }

/// <summary>0 (Any) means "do not filter on origin", so default(RequestFilter) is a valid match-all.</summary>
public enum OriginMask { Any = 0, Original = 1, Derived = 2 }
