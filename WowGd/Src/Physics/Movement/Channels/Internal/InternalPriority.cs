using System;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

[Flags]
public enum InternalPriority
{
    Grounded     = 0b0100,
    PriorityMask = 0b0011,
}