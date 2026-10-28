using System.Numerics;

/// Open tag sets, with independant registries per domain.
/// 
/// - A domain is a simple discrimination marker. TagRegistry<TDomain> is a static class, so each domain gets
/// its own static state, and Tag<A>/Tag<B> cannot be mixed by mistake.
/// - Tags are hierarchical in a dot notation - "Spell.Fire" registers as both "Spell" and "Spell.Fire".
/// Said otherwise, a set that only contains "Spell.Fire" implictly contains "Spell".
/// - Tags are registered under an index, but in a tag set, they are packed in growing masks, built with chunks of 64 bits.
/// The total number of chunks doesn't grow itself, the set uses the number of chunk they need for individual operations.
namespace CombatCore.Primitives.Tags;


/// <summary>
/// Immutable set of tags: ulong chunks, sized by the highest tag it contains, so default is the empty set and sets built
/// before the registry grew still compare correctly with later ones (a missing chunk reads as zero).
/// The set stores the closure: adding Spell.Fire also sets Spell. Without(tag) removes the tag and its descendants;
/// ancestors stay (removing the only Spell.* child leaves the category Spell).
/// </summary>
public readonly struct TagSet<TDomain> : IEquatable<TagSet<TDomain>>
{
    private readonly ulong[]? _chunks;   // canonical: no trailing zero chunk, null when empty

    private TagSet(ulong[]? chunks) => _chunks = chunks;

    public static TagSet<TDomain> Empty => default;
    public bool IsEmpty => _chunks is null;

    public static TagSet<TDomain> Of(params Tag<TDomain>[] tags)
    {
        if (tags.Length == 0) return default;
        var chunks = new ulong[(tags.Max(t => t.Index) >> 6) + 1];
        foreach (var tag in tags) SetClosure(chunks, tag.Index);
        return new(chunks);
    }

    public TagSet<TDomain> With(Tag<TDomain> tag)
    {
        var chunks = Grow(Math.Max(_chunks?.Length ?? 0, (tag.Index >> 6) + 1));
        SetClosure(chunks, tag.Index);
        return new(chunks);
    }

    public TagSet<TDomain> Without(Tag<TDomain> tag)
    {
        if (_chunks is null) return this;
        var chunks = (ulong[])_chunks.Clone();
        foreach (var index in Indices(_chunks))
            if (TagRegistry<TDomain>.IsSameOrDescendant(index, tag.Index))
                chunks[index >> 6] &= ~(1UL << (index & 63));
        return new(Trim(chunks));
    }

    public TagSet<TDomain> Union(TagSet<TDomain> other)
    {
        if (other._chunks is null) return this;
        if (_chunks is null) return other;
        var chunks = Grow(Math.Max(_chunks.Length, other._chunks.Length));
        for (var i = 0; i < other._chunks.Length; i++) chunks[i] |= other._chunks[i];
        return new(chunks);
    }

    public bool Contains(Tag<TDomain> tag)
    {
        var chunk = tag.Index >> 6;
        return _chunks is not null && chunk < _chunks.Length && (_chunks[chunk] & (1UL << (tag.Index & 63))) != 0;
    }

    /// <summary>Every tag of other is in this set (hierarchy included: a set with Spell.Fire contains all of {Spell}).</summary>
    public bool ContainsAll(TagSet<TDomain> other)
    {
        if (other._chunks is null) return true;
        for (var i = 0; i < other._chunks.Length; i++)
        {
            var mine = _chunks is not null && i < _chunks.Length ? _chunks[i] : 0UL;
            if ((other._chunks[i] & ~mine) != 0) return false;
        }
        return true;
    }

    public bool Intersects(TagSet<TDomain> other)
    {
        if (_chunks is null || other._chunks is null) return false;
        for (var i = 0; i < Math.Min(_chunks.Length, other._chunks.Length); i++)
            if ((_chunks[i] & other._chunks[i]) != 0) return true;
        return false;
    }

    /// <summary>Every tag in the set, ancestors included.</summary>
    public IEnumerable<Tag<TDomain>> All() =>
        _chunks is null ? [] : Indices(_chunks).Select(i => new Tag<TDomain>(i));

    /// <summary>The most specific tags: the ones with no descendant in the set. What you want to display.</summary>
    public IEnumerable<Tag<TDomain>> Leaves()
    {
        if (_chunks is null) return [];
        var hasChild = new HashSet<int>();
        foreach (var index in Indices(_chunks))
            if (TagRegistry<TDomain>.ParentOf(index) is var p and >= 0) hasChild.Add(p);
        return Indices(_chunks).Where(i => !hasChild.Contains(i)).Select(i => new Tag<TDomain>(i)).ToList();
    }

    public bool Equals(TagSet<TDomain> other) =>
        (_chunks ?? []).AsSpan().SequenceEqual((other._chunks ?? []).AsSpan());
    public override bool Equals(object? obj) => obj is TagSet<TDomain> other && Equals(other);
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var chunk in _chunks ?? []) hash.Add(chunk);
        return hash.ToHashCode();
    }
    public static bool operator ==(TagSet<TDomain> a, TagSet<TDomain> b) => a.Equals(b);
    public static bool operator !=(TagSet<TDomain> a, TagSet<TDomain> b) => !a.Equals(b);
    public override string ToString() => IsEmpty ? "{}" : "{" + string.Join(", ", Leaves()) + "}";

    // ---- helpers ----
    private ulong[] Grow(int length)
    {
        var chunks = new ulong[length];
        _chunks?.CopyTo(chunks, 0);
        return chunks;
    }

    private static void SetClosure(ulong[] chunks, int index)
    {
        // parents have smaller indices than their children by construction,
        // so they fit in the chunks already sized for the child
        for (var i = index; i >= 0; i = TagRegistry<TDomain>.ParentOf(i))
            chunks[i >> 6] |= 1UL << (i & 63);
    }

    private static ulong[]? Trim(ulong[] chunks)
    {
        var length = chunks.Length;
        while (length > 0 && chunks[length - 1] == 0) length--;
        if (length == 0) return null;
        return length == chunks.Length ? chunks : chunks[..length];
    }

    private static IEnumerable<int> Indices(ulong[] chunks)
    {
        for (var c = 0; c < chunks.Length; c++)
        {
            var word = chunks[c];
            while (word != 0)
            {
                var bit = BitOperations.TrailingZeroCount(word);
                yield return (c << 6) + bit;
                word &= word - 1;
            }
        }
    }
}