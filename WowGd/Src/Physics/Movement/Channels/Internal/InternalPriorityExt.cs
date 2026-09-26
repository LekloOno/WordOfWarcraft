namespace WowGd.Src.Physics.Movement.Channels.Internal;

public static class InternalPriorityExt
{
    public static InternalPriority Layer(this InternalPriority priority) =>
        priority & InternalPriority.LayerMask;

    public static bool Grounded(this InternalPriority priority) =>
        priority.HasFlag(InternalPriority.Grounded);

    public static bool Base(this InternalPriority priority) =>
        priority.Layer() is InternalPriority.Base;

    public static bool Tackle(this InternalPriority priority) =>
        priority.Layer() is InternalPriority.Tackle;

    public static bool Override(this InternalPriority priority) =>
        priority.Layer() is InternalPriority.Override;
}