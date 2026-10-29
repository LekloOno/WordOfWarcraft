using CombatCore.Sources.Attachment;

namespace CombatCore.Sources.Lifecycle;

public sealed partial class SourcesProcessor
{
    public void Insert(Source parent, Source child, object? ctx = null)
        => Submit(new(SourceCommandKind.Insert, parent, child, 0, ctx));

    public void Terminate(Source node, object? ctx = null)
        => Submit(new(SourceCommandKind.Terminate, node, null, 0, ctx));

    public void Kill(Source node, object? ctx = null)
        => Submit(new(SourceCommandKind.Kill, node, null, 0, ctx));

    public void Abort(Source node, object? ctx = null)
        => Submit(new(SourceCommandKind.Abort, node, null, 0, ctx));

    internal void CompleteTermination(TerminationTicket ticket, object? ctx)
        => Submit(new(SourceCommandKind.CompleteTermination, ticket.Node, null, ticket.Generation, ctx));
}