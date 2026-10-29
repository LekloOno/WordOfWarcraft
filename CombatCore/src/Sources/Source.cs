using CombatCore.Sources.Attachment;
using CombatCore.Sources.Lifecycle;

namespace CombatCore.Sources;

public sealed class Source(uint id, string? name = null, SourceTagSet? tagSet = null, ISourceAttachment? attachment = null)
{
    public readonly uint Id = id;
    public readonly string Name = name ?? string.Empty;
    public SourceTagSet Tags { get; } = tagSet ?? SourceTagSet.Empty;
    public SourceTagSet InheritedTags { get; internal set; } = tagSet ?? SourceTagSet.Empty;
    public SourceState State { get; internal set; } = SourceState.Terminated;
    public ISourceAttachment? Attachment { get; } = attachment;
    public Source? Parent { get; internal set; }
    public IReadOnlyList<Source> Children => MutChildren;
 
    // --- processor bookkeeping (internal on purpose) ---
    internal readonly List<Source> MutChildren = [];
 
    /// <summary>Bumped on every pending-entry / abort / finalize. Makes stale tickets harmless.</summary>
    internal int Generation;
 
    /// <summary>True while the attachment still has to call ticket.Complete().</summary>
    internal bool AwaitingAttachment;
 
    /// <summary>The node whose Terminate command started the pending wave this node is in (itself, or an ancestor).</summary>
    public Source? TerminationRoot { get; internal set; }

    public override string ToString() => $"{Name}:{Id}";
}
