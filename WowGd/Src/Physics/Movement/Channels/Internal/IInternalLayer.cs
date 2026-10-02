using Godot;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public interface IInternalLayer
{
    Contribution GetContribution(Vector2 wishDir, EntityMover mover, float delta);
    void OnChannelClosed();
    void OnChannelOpened();

    IInternalContributor? SetContributor(int index, IInternalContributor contributor);
    IInternalContributor? UnsetContributor(int index);

    IInternalContributor? Active {get;}
}