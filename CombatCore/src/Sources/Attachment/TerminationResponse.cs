namespace CombatCore.Sources.Attachment;

public enum TerminationResponse
{
    /// <summary>Nothing to wait for: the processor completes the termination right away.</summary>
    Done,
    /// <summary>The attachment keeps the ticket and calls ticket.Complete() when it is really done.</summary>
    Deferred,
}