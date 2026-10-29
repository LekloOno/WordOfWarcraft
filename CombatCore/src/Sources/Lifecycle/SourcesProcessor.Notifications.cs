using CombatCore.Sources.Attachment;

namespace CombatCore.Sources.Lifecycle;

// Notifications happens after mutation
public sealed partial class SourcesProcessor
{
    private enum NoteKind { Inserted, Removed, TerminationStarted, Aborted, Killed, Terminated }

    private readonly record struct Notification(NoteKind Kind, Source Node, Source? Other, int Generation, bool Flag);

    private void Note(NoteKind kind, Source node, Source? other, int generation = 0, bool flag = false)
        => _notes.Add(new(kind, node, other, generation, flag));

    private void Flush(object? ctx)
    {
        // Hooks may submit commands: they are only queued while we drain, so _notes is not modified here.
        for (int i = 0; i < _notes.Count; i++)
        {
            var n = _notes[i];
            switch (n.Kind)
            {
                case NoteKind.Inserted: // Node = child, Other = parent
                    n.Other!.Attachment?.ChildInserted(n.Node, ctx);
                    n.Node.Attachment?.InsertedTo(n.Other, ctx);
                    _sink.OnInserted(n.Other, n.Node);
                    break;

                case NoteKind.Removed: // Node = child, Other = former parent
                    n.Other!.Attachment?.ChildRemoved(n.Node, ctx);
                    n.Node.Attachment?.RemovedFrom(n.Other, ctx);
                    break;

                case NoteKind.TerminationStarted:
                    _sink.OnTerminationStarted(n.Node);
                    if (n.Flag)
                    {
                        var ticket = new TerminationTicket(this, n.Node, n.Generation);
                        var response = n.Node.Attachment!.TerminationRequested(ticket, ctx);
                        if (response is TerminationResponse.Done)
                            ticket.Complete(ctx);
                    }
                    break;

                case NoteKind.Aborted:
                    _sink.OnAborted(n.Node);
                    n.Node.Attachment?.TerminationAborted(ctx);
                    break;

                case NoteKind.Killed:
                    n.Node.Attachment?.Killed(ctx);
                    break;

                case NoteKind.Terminated:
                    _sink.OnTerminated(n.Node, killed: n.Flag);
                    break;
            }
        }
        _notes.Clear();
    }
}