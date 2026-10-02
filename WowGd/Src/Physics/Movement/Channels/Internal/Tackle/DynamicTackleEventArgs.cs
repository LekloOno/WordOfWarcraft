using System;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public sealed class DynamicTackledEventArgs(TackledEventArgs tackledEventArgs) : EventArgs
{
    public DynamicTackleNode Tackler { get; } = tackledEventArgs.Tackler.Entity.EntityMover.DynamicTackleNode;
    public int TacklerCount { get; } = tackledEventArgs.TacklerCount;
}