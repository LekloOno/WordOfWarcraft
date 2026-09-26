using Godot;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public abstract partial class InternalLayer : Node, IInternalLayer
{
    [Export] public float FrictionBase { get; private set;}
    /// <summary>
    /// Below this speed, and when WishDir is aligned with the entity velocity, friction can be discarded.
    /// </summary>
    [Export] public float MaxSpeed { get; private set;}
    public abstract IWishDir WishDir { get; }

    public Contribution GetContribution(EntityMover mover, float delta)
    {
        GetForce(mover, delta, out Vector2? accel, out Vector2? raw);
        return new(accel, raw);
    }

    protected abstract void GetForce(EntityMover mover, float delta, out Vector2? accel, out Vector2? raw);

    public abstract void OnChannelClosed();
    public abstract void OnChannelOpened();
}