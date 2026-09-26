namespace WowGd.Src.Physics.Movement.Channels.Internal;

public class InternalContributor : IContributor
{
    private InternalLayer? _override;
    private InternalLayer? _tackle;
    private InternalLayer _base = null!;

    public InternalLayer Current { get; private set; } = null!;

    public void SetOverride(InternalLayer @override)
    {
        _override = @override;
        Current = @override;
    }

    public void UnsetOverride()
    {
        if (Current == _override)
            Current = _tackle ?? _base;

        _override = null;
    }

    public void SetTackle(InternalLayer tackle)
    {
        _tackle = tackle;

        if (Current != _override)
            Current = tackle;
    }

    public void UnsetTackle()
    {
        if (Current == _tackle)
            Current = _base;

        _tackle = null;
    }

    public void SetBase(InternalLayer @base)
    {
        if (Current == _base)
            Current = @base;
            
        _base = @base;
    }

    public Contribution GetContribution(EntityMover mover, float delta) =>
        Current.GetContribution(mover, delta);

    public void OnChannelClosed()
    {
        _override?.OnChannelClosed();
        _tackle?.OnChannelClosed();
        _base.OnChannelClosed();
    }

    public void OnChannelOpened()
    {
        _override?.OnChannelOpened();
        _tackle?.OnChannelOpened();
        _base.OnChannelOpened();
    }
}