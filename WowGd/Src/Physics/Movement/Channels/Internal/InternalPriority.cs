using System;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

[Flags]
public enum InternalPriority
{
    Grounded    = 0b0001,

    // Modes cannot be combined.
    // To ensure this, and not rely on the user properly using the flags :D
    //  we enforce them through the bit structure.
    // -
    // So they should not be matched with .HasFlag
    // They are matched stricly, using the LayerMask.
    //
    // The active layer is the layer that matches exactly on "Priority & LayerMask".
    Base        = 0b0010,
    Tackle      = 0b0100,
    Override    = 0b0110,

    LayerMask   = 0b0110
}