namespace WowGd.Src.Physics.Movement.Channels.Internal;

public static class InternalPriorityExt
{
    public static bool Grounded(this InternalPriority priority) =>
        priority.HasFlag(InternalPriority.Grounded);

    public static int PriorityIndex(this InternalPriority priority) =>
        (int) (priority & InternalPriority.PriorityMask);

    public static InternalPriority From(bool grounded, int priority) =>
        (InternalPriority) (priority | (grounded ? (int) InternalPriority.Grounded : 0));
}