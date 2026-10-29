using CombatCore.Sources.Lifecycle;

namespace CombatCore.Sources.Attachment;

/// <summary>
/// Handed to the attachment when its source enters PendingTermination.
/// Completing a ticket that is no longer current (aborted, killed, finalized) is a harmless no-op.
/// </summary>
public readonly record struct TerminationTicket(SourcesProcessor Processor, Source Node, int Generation)
{
    public void Complete(object? ctx = null) => Processor.CompleteTermination(this, ctx);
}