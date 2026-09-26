using Godot;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Physics.Movement.Internal;

public abstract partial class InternalMovementLayer : Node, IInternalMovementLayer
{
    [Export] public float FrictionBase { get; private set;}
    /// <summary>
    /// Below this speed, and when WishDir is aligned with the entity velocity, friction can be discarded.
    /// </summary>
    [Export] public float MaxSpeed { get; private set;}
    public abstract IWishDir WishDir { get; }

    public void GetContribution(EntityMover mover, float delta, out Vector2 force, out float frictionRatio)
    {
        force = GetForce(mover, delta);
        frictionRatio = 1f;
    }

    protected abstract Vector2 GetForce(EntityMover mover, float delta);

    public abstract void OnChannelClosed();
    public abstract void OnChannelOpened();
}