using CombatCore.Primitives;

namespace CombatCore.Sources;

/// <summary>Read-only questions on the tree. They replace Source.Kind / Source.Id / Source.Owner.</summary>
public static class SourceQueries
{
    /// <summary>
    /// The node itself, or a descendant of <paramref name="ancestor"/>.
    /// A child inherits every tag of its parent when it is inserted, so a descendant always holds all the inherited tags of its
    /// ancestors: if it does not, it can not be below, and the climb is skipped. (Relies on tags being fixed while a node is in the
    /// tree. A node with no tag at all can not short-circuit and falls back to the climb.)
    /// </summary>
    public static bool IsSameOrDescendantOf(this Source node, Source ancestor)
    {
        if (node == ancestor) return true;
        if (!node.InheritedTags.ContainsAll(ancestor.InheritedTags)) return false;

        for (var n = node.Parent; n is not null; n = n.Parent)
            if (n == ancestor) return true;
        return false;
    }

    /// <summary>Nearest attachment of type T, looking at the node first, then up. The "does the spell implement X" mechanism.</summary>
    public static T? FindAttachment<T>(this Source node) where T : class
    {
        for (var n = node; n is not null; n = n.Parent)
            if (n.Attachment is T found) return found;
        return null;
    }

    /// <summary>Whose is this source? (the entity node above it)</summary>
    public static EntityId? OwnerOf(this Source node) => node.FindAttachment<EntityAttachment>()?.Id;
}
