namespace CombatCore.Sources;

/// <summary>
/// Tags of the nodes the core itself builds (entities, status instances). Spells, abilities, passives... are not the core's business:
/// whoever defines them registers their own tags. A node's InheritedTags also hold its ancestors' tags, so "this comes from an
/// entity" is simply <c>InheritedTags.Contains(SourceTags.Entity)</c>, whatever the depth of the node.
/// Registered on first use: if the game bulk-registers the source domain (RegisterAll) or freezes it,
/// it must include these names (or touch this class first).
/// </summary>
public static class SourceTags
{
    public static readonly SourceTag Entity = SourceTag.Register("Entity");
    public static readonly SourceTag Status = SourceTag.Register("Status");      // one status instance on an entity
}
