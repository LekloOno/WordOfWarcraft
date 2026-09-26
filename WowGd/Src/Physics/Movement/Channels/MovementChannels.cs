using System;

namespace WowGd.Src.Physics.Movement.Channels;

[Flags]
public enum MovementChannels
{
    /// <summary>
    /// An unhindereable channel.
    /// </summary>
    God = 1,
    Internal = 1 << 1,
    External = 1 << 2,
    Warp     = 1 << 3,
}