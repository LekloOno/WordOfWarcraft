namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public readonly struct TackleInteraction(TackledEventArgs tackledEventArgs, TackleInteractionType type)
{
    public DynamicTackleNode Tackler { get; } = tackledEventArgs.Tackler.Entity.EntityMover.DynamicTackleNode;
    public TackleInteractionType Type { get; } = type;
    public int TacklerCount { get; } = tackledEventArgs.TacklerCount;
}

public enum TackleInteractionType
{
    /// <summary>
    /// A tackle interraction just started.
    /// </summary>
    Initialized,
    /// <summary>
    /// An additional tackler joined the interraction.
    /// </summary>
    Tackled,
    /// <summary>
    /// A tackler left the interraction.
    /// </summary>
    Released,
    /// <summary>
    /// Last tackler released his tackle, or distance broke all tackles links.
    /// </summary>
    Freed,
    /// <summary>
    /// Tackle timed out.
    /// </summary>
    Dodged,
}