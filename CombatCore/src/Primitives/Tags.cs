namespace CombatCore.Primitives;

/// <summary>
/// Essentially a core-side StringName.
/// 
/// Open set, declared by name anywhere, compared as a small int at runtime.
/// The index depends on registration order: never persist or send it raw (exchange names at handshake).
/// </summary>
public readonly record struct Tag(int Index)
{
    static readonly Dictionary<string, Tag> ByName = [];
    static readonly List<string> Names = [];

    public static Tag Register(string name)
    {
        lock (ByName)
        {
            if (ByName.TryGetValue(name, out var existing)) return existing;
            if (Names.Count >= 64) throw new InvalidOperationException("Skeleton limit: 64 tags (ulong bitset).");
            var tag = new Tag(Names.Count);
            Names.Add(name);
            ByName[name] = tag;
            return tag;
        }
    }

    public string Name => Names[Index];
}

public readonly record struct TagSet(ulong Bits)
{
    public static TagSet Of(params Tag[] tags) => tags.Aggregate(default(TagSet), (s, t) => s.With(t));
    public TagSet With(Tag t) => new(Bits | (1UL << t.Index));
    public bool Contains(Tag t) => (Bits & (1UL << t.Index)) != 0;
    public bool ContainsAll(TagSet o) => (Bits & o.Bits) == o.Bits;
    public bool Intersects(TagSet o) => (Bits & o.Bits) != 0;
}
