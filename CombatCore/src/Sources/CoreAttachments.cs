using CombatCore.Primitives;
using CombatCore.Sources.Attachment;

namespace CombatCore.Sources;

// The authority never asks "what is this node". Interceptors and handlers do, by climbing the tree
// and looking at attachments (see SourceQueries). Only entities are the core's own business.

/// <summary>The node of an entity. Its subtree is "everything that entity owns / caused".</summary>
public sealed class EntityAttachment(EntityId id) : ISourceAttachment
{
    public EntityId Id { get; } = id;
}
