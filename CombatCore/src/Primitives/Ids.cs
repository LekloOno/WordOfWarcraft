namespace CombatCore.Primitives;

// Plain data only: everything here must be serializable later (facts are the wire protocol).
public readonly record struct EntityId(uint Value)   { public override string ToString() => $"E{Value}"; }
public readonly record struct RequestId(uint Value)  { public override string ToString() => $"r{Value}"; }

/// <summary>Open id (the game side defines the constants). The core never needs to know which one is "health".</summary>
public readonly record struct ResourceId(int Value);

public enum Origin { Original, Derived }

/// <summary>0 (Any) means "do not filter on origin", so default(RequestFilter) is a valid match-all.</summary>
public enum OriginMask { Any = 0, Original = 1, Derived = 2 }
