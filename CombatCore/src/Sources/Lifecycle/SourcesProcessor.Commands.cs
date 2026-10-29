namespace CombatCore.Sources.Lifecycle;

// mutation only, never call hooks
public sealed partial class SourcesProcessor
{

    private void ExecuteInsert(Source parent, Source child)
    {
        if (parent.State is SourceState.Terminated)
        { Reject(SourceCommandKind.Insert, parent, SourceRejectReason.ParentNotInTree); return; }

        // A Terminated node is never in the tree, so this also rules out cycles and double insertion.
        if (child.State is not SourceState.Terminated)
        { Reject(SourceCommandKind.Insert, child, SourceRejectReason.ChildAlreadyInTree); return; }

        parent.MutChildren.Add(child);
        child.Parent = parent;
        child.InheritedTags = child.Tags.Union(parent.InheritedTags);
        Note(NoteKind.Inserted, child, parent);

        if (parent.State is SourceState.Active)
        {
            child.State = SourceState.Active;
            return;
        }

        // Parent is pending: the child is born pending, in the same wave as its parent.
        _scratch.Clear();
        MarkPending(child, parent.TerminationRoot!);
        TryFinalize(child);
    }

    private void ExecuteTerminate(Source node)
    {
        if (node == Root) { Reject(SourceCommandKind.Terminate, node, SourceRejectReason.IsRoot); return; }
        if (node.State is not SourceState.Active) return; // already pending or not in tree: idempotent

        _scratch.Clear();
        MarkPending(node, node);

        // _scratch is pre-order, so walking it backwards visits children before parents.
        for (int i = _scratch.Count - 1; i >= 0; i--)
            TryFinalize(_scratch[i]);
    }

    private void ExecuteKill(Source node)
    {
        if (node == Root) { Reject(SourceCommandKind.Kill, node, SourceRejectReason.IsRoot); return; }
        if (node.State is SourceState.Terminated) return;

        var parent = node.Parent;

        _scratch.Clear();
        CollectPreOrder(node);
        for (int i = _scratch.Count - 1; i >= 0; i--)
            Detach(_scratch[i], killed: true);

        // The parent may have been waiting only for this node.
        TryFinalize(parent);
    }

    private void ExecuteAbort(Source node)
    {
        if (node.State is not SourceState.PendingTermination) return;

        if (node.TerminationRoot != node)
        { Reject(SourceCommandKind.Abort, node, SourceRejectReason.NotTerminationRoot); return; }

        Revert(node, node);
    }

    private void ExecuteComplete(Source node, int generation)
    {
        if (node.State is not SourceState.PendingTermination || !node.AwaitingAttachment || node.Generation != generation)
        { Reject(SourceCommandKind.CompleteTermination, node, SourceRejectReason.StaleTicket); return; }

        node.AwaitingAttachment = false;

        // Completing may release the attachment-less descendants it was holding: re-check the whole subtree, leaves first.
        _scratch.Clear();
        CollectPreOrder(node);
        for (int i = _scratch.Count - 1; i >= 0; i--)
            TryFinalize(_scratch[i]);
    }
}