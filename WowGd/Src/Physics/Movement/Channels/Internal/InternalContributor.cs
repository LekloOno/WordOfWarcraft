using System;
using Godot;
using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public abstract partial class InternalContributor : Node, IInternalContributor
{
    [Export] public float FrictionBase { get; private set;}
    /// <summary>
    /// Below this speed, and when WishDir is aligned with the entity velocity, friction can be discarded.
    /// </summary>
    [Export] public float BaseSpeed { get; private set; } = 2.5f;
    
    public float MaxSpeed => MathF.Min(BaseSpeed, DynamicTackleNode.EffectiveLimit);
    public Vector2 WishDir { private get; set; }

    public Contribution GetContribution(EntityMover mover, float delta)
    {
        GetForces(new(mover, MaxSpeed, WishDir), delta, out Vector2? accel, out Vector2? raw);
        return new(accel, raw);
    }

    public abstract DynamicTackleNode DynamicTackleNode { get; }
    protected abstract void GetForces(InternalLayerInput input, float delta, out Vector2? accel, out Vector2? raw);

    public abstract void OnChannelClosed();
    public abstract void OnChannelOpened();
}