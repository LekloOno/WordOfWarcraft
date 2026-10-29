namespace CombatCore.Sources.Lifecycle;

public sealed partial class SourcesProcessor
{
    /// <summary>Marks node and its Active descendants pending (pure mutation, no finalization yet).</summary>
    private void MarkPending(Source node, Source root)
    {
        node.State = SourceState.PendingTermination;
        node.Generation++;
        node.TerminationRoot = root;
        node.AwaitingAttachment = node.Attachment is not null;
        _scratch.Add(node);
        Note(NoteKind.TerminationStarted, node, null, node.Generation, node.AwaitingAttachment);

        foreach (var child in node.MutChildren)
            if (child.State is SourceState.Active)
                MarkPending(child, root);
    }

    /// <summary>Finalizes node if it is done, then checks its parent, and so on upwards.</summary>
    private void TryFinalize(Source? node)
    {
        while (node is not null
               && node.State is SourceState.PendingTermination
               && node.MutChildren.Count == 0
               && !node.AwaitingAttachment
               && !IsHeld(node))
        {
            var parent = node.Parent;
            Detach(node, killed: false);
            node = parent;
        }
    }

    /// <summary>
    /// An attachment-less source has no lifetime of its own: it is held as long as one of its pending
    /// ancestors is still waiting for its attachment. (Active ancestors stop the walk: nobody holds the wave open.)
    /// </summary>
    private static bool IsHeld(Source node)
    {
        if (node.Attachment is not null) return false;

        for (var p = node.Parent; p is not null && p.State is SourceState.PendingTermination; p = p.Parent)
            if (p.AwaitingAttachment) return true;

        return false;
    }

    private void Detach(Source node, bool killed)
    {
        var parent = node.Parent!;

        node.State = SourceState.Terminated;
        node.Generation++;
        node.AwaitingAttachment = false;
        node.TerminationRoot = null;

        parent.MutChildren.Remove(node);
        node.Parent = null;
        node.InheritedTags = node.Tags;

        if (killed) Note(NoteKind.Killed, node, null);
        Note(NoteKind.Removed, node, parent);
        Note(NoteKind.Terminated, node, null, flag: killed);
    }

    private void Revert(Source node, Source root)
    {
        node.State = SourceState.Active;
        node.Generation++;
        node.AwaitingAttachment = false;
        node.TerminationRoot = null;
        Note(NoteKind.Aborted, node, null);

        // Only the nodes pending because of THIS wave come back; a child terminating for its own reason stays pending.
        foreach (var child in node.MutChildren)
            if (child.State is SourceState.PendingTermination && child.TerminationRoot == root)
                Revert(child, root);
    }

    private void CollectPreOrder(Source node)
    {
        _scratch.Add(node);
        foreach (var child in node.MutChildren) CollectPreOrder(child);
    }

    private void Reject(SourceCommandKind kind, Source node, SourceRejectReason reason)
        => _sink.OnRejected(kind, node, reason);
}