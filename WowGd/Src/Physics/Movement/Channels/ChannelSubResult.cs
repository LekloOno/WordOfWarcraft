using System;

namespace WowGd.Src.Physics.Movement.Channels;

[Flags]
public enum ChannelSubResult
{
    None            = 0b00,
    /// <summary>
    /// Whether the contributor was successfully added/removed.
    /// </summary>
    Success         = 0b01,
    /// <summary>
    /// Whether the channel the contributor was added to is opened.
    /// </summary>
    ChannelOpened   = 0b10,
    Both            = 0b11,
}