using Godot;
using WowGd.Src.Physics.Movement.Channels;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Physics.Movement.Internal;

public class InternalMovement : IMovementContributor
{
    public readonly InternalMovementLayers GroundInternal = new();
    public readonly InternalMovementLayers AirInternal = new();
    private InternalMovementLayers _currentInternal = null!;

    public float FrictionBase => _currentInternal.Current.FrictionBase;
    public float MaxSpeed => _currentInternal.Current.MaxSpeed;
    public IWishDir WishDir => _currentInternal.Current.WishDir;

    public void GetContribution(EntityMover mover, float delta, out Vector2 force, out float frictionRatio) =>
        _currentInternal.GetContribution(mover, delta, out force, out frictionRatio);

    public void OnChannelClosed() =>
        _currentInternal.OnChannelClosed();

    public void OnChannelOpened() =>
        _currentInternal.OnChannelOpened();

    public void SetGrounded() =>
        _currentInternal = GroundInternal;

    public void SetAirborne() =>
        _currentInternal = AirInternal;
}