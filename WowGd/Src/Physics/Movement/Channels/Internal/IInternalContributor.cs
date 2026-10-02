using Godot;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public interface IInternalContributor : IContributor
{
    float FrictionBase { get; } 
    /// <summary>
    /// Below this speed, and when WishDir is aligned with the entity velocity, friction can be discarded.
    /// </summary>
    float MaxSpeed { get; }
    Vector2 WishDir { set; }
}