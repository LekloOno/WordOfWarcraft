using Godot;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public abstract partial class InternalContributor : Node, IInternalContributor
{
    public abstract float FrictionBase { get; }
    public abstract float MaxSpeed { get; }
    public Vector2 WishDir { private get; set; }

    public Contribution GetContribution(EntityMover mover, float delta)
    {
        GetForces(new(mover, MaxSpeed, WishDir), delta, out Vector2? accel, out Vector2? raw);
        return new(accel, raw);
    }

    protected abstract void GetForces(InternalLayerInput input, float delta, out Vector2? accel, out Vector2? raw);

    public abstract void OnChannelClosed();
    public abstract void OnChannelOpened();
}