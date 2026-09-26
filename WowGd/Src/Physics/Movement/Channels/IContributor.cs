namespace WowGd.Src.Physics.Movement.Channels;

public interface IContributor
{
    Contribution GetContribution(EntityMover mover, float delta);

    /// <summary>
    /// Called when the channel this contributor is bound to is closed.
    /// </summary>
    void OnChannelClosed();
    /// <summary>
    /// Called when the channel this contributor is bound to is opened.
    /// </summary>
    void OnChannelOpened();
}