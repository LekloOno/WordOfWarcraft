namespace CombatCore.Sources.Lifecycle;

public enum SourceState
{
    /// <summary>Not in the tree. Never has children. Can be inserted (again).</summary>
    Terminated,
    Active,
    /// <summary>In the tree, draining: waiting for its children and its attachment.</summary>
    PendingTermination,
}

public enum SourceCommandKind
{
    Insert,
    Terminate,
    Kill,
    Abort,
    CompleteTermination
}

public enum SourceRejectReason
{
    ParentNotInTree,       // Insert: parent is Terminated
    ChildAlreadyInTree,    // Insert: child is not Terminated
    IsRoot,                // Terminate / Kill on the root
    NotTerminationRoot,    // Abort on a node that is pending only because an ancestor terminates
    StaleTicket,           // CompleteTermination with an outdated ticket
}

/// <summary>
/// One lifecycle command. Insert: Node = the parent, Other = the child. Every other kind: Node = the node concerned.
/// Generation is only used by CompleteTermination (stale tickets are ignored). Ctx is the arbitrary payload given to the hooks.
/// </summary>
public readonly record struct SourceCommand(SourceCommandKind Kind, Source Node, Source? Other, int Generation, object? Ctx);
