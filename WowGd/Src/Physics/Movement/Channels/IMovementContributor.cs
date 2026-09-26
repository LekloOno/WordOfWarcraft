using Godot;

namespace WowGd.Src.Physics.Movement.Channels;

public interface IMovementContributor
{
    void GetContribution(
        EntityMover mover, 
        float delta, 
        out Vector2 force, 
        out float frictionRatio
    );

    /// <summary>
    /// Called when the channel this contributor is bound to is closed.
    /// </summary>
    void OnChannelClosed();
    /// <summary>
    /// Called when the channel this contributor is bound to is opened.
    /// </summary>
    void OnChannelOpened();
}