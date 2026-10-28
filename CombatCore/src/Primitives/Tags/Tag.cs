namespace CombatCore.Primitives.Tags;

public readonly record struct Tag<TDomain>(int Index)
{
    public static Tag<TDomain> Register(string name) => TagRegistry<TDomain>.Register(name);
    public static bool TryFind(string name, out Tag<TDomain> tag) => TagRegistry<TDomain>.TryFind(name, out tag);

    public string Name => TagRegistry<TDomain>.NameOf(Index);

    /// <summary>"Spell" for "Spell.Fire", null for a root.</summary>
    public Tag<TDomain>? Parent => TagRegistry<TDomain>.ParentOf(Index) is var p and >= 0 ? new Tag<TDomain>(p) : null;

    /// <summary>True for the tag itself and for every tag registered below it.</summary>
    public bool IsSameOrDescendantOf(Tag<TDomain> ancestor) => TagRegistry<TDomain>.IsSameOrDescendant(Index, ancestor.Index);

    public override string ToString() => Name;
}