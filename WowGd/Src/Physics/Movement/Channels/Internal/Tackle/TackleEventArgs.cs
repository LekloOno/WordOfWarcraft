using System;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public sealed class TackledEventArgs(TackleNode tackler, int tacklerCount) : EventArgs
{
    public TackleNode Tackler { get; } = tackler;
    public int TacklerCount { get; } = tacklerCount;
}