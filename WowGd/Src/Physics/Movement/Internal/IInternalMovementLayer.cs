using WowGd.Src.Physics.Movement.Channels;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Physics.Movement.Internal;

public interface IInternalMovementLayer : IMovementContributor
{
    float FrictionBase { get; } 
    /// <summary>
    /// Below this speed, and when WishDir is aligned with the entity velocity, friction can be discarded.
    /// </summary>
    float MaxSpeed { get; }
    IWishDir WishDir { get; }
}