using System.Collections.Generic;

namespace WowGd.Src.Physics.Movement.Channels.Additive;

public class AdditiveChannel : IContributorChannel
{
    private readonly List<IContributor> _contributors = [];

    public Contribution GetContribution(EntityMover mover, float delta)
    {
        Contribution contrib = new();
        foreach (IContributor contributor in _contributors)
            contrib += contributor.GetContribution(mover, delta);

        return contrib;
    }

    public void Open()
    {
        foreach (IContributor contributor in _contributors)
            contributor.OnChannelOpened();
    }

    public void Close()
    {
        foreach (IContributor contributor in _contributors)
            contributor.OnChannelClosed();
    }

    public bool AddContributor(IContributor contributor, uint priority)
    {
        _contributors.Add(contributor);
        return true;
    }

    public bool RemoveContributor(IContributor contributor)
    {
        int index = _contributors.IndexOf(contributor);

        if (index < 0)
            return false;
        
        _contributors[index] = _contributors[^1];
        _contributors.RemoveAt(_contributors.Count - 1);
        return true;
    }
}