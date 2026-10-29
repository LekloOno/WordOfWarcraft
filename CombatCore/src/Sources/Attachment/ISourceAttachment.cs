namespace CombatCore.Sources.Attachment;

/// <summary>
/// All hooks are called by the processor AFTER the tree mutation of the current command is done,
/// so a hook always sees a consistent tree. Commands submitted from a hook are queued, never nested.
/// Default implementations: implement only what you care about.
/// </summary>
public interface ISourceAttachment
{
    void ChildInserted(Source child, object? ctx) { }
    void InsertedTo(Source parent, object? ctx) { }
    void ChildRemoved(Source child, object? ctx) { }
    void RemovedFrom(Source parent, object? ctx) { }
 
    /// <summary>
    /// The source entered PendingTermination (also when it is born pending).
    /// Contract: once told to terminate, an attachment that spawns children (periodic AoE, wave sequence...) stops
    /// spawning, and calls ticket.Complete() when it is really done. Nothing enforces it (a timeout kill may come later).
    /// Attachment-less descendants stay alive until this ticket completes.
    /// </summary>
    TerminationResponse TerminationRequested(TerminationTicket ticket, object? ctx) => TerminationResponse.Done;
 
    /// <summary>The pending termination of this source was aborted; it is Active again.</summary>
    void TerminationAborted(object? ctx) { }
 
    /// <summary>Hard kill. Synchronous, cannot defer. Release resources here.</summary>
    void Killed(object? ctx) { }
}
