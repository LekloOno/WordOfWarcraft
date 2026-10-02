using Godot;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public class InternalLayer : IInternalLayer
{
    public IInternalContributor? Active { get; private set; } = null;
    
    private readonly IInternalContributor?[] _layers = new IInternalContributor[8];
    private int _topIndex = -1;

    public InternalLayer() {}
    public InternalLayer(IInternalContributor @base) : this()
    {
        _layers[0] = @base;
        Active = @base;
        _topIndex = 0;
    }

    public Contribution GetContribution(Vector2 wishDir, EntityMover mover, float delta)
    {
        if (Active is null)
            return new();

        Active.WishDir = wishDir;
        return Active.GetContribution(mover, delta);
    }

    public void OnChannelClosed()
    {
        foreach (IInternalContributor? layer in _layers)
            layer?.OnChannelClosed();
    }

    public void OnChannelOpened()
    {
        foreach (IInternalContributor? layer in _layers)
            layer?.OnChannelOpened();
    }

    public IInternalContributor? SetContributor(int index, IInternalContributor contributor)
    {
        IInternalContributor? prev = _layers[index];
        _layers[index] = contributor;

        if (index < _topIndex)
            return prev;

        Active = contributor;
        _topIndex = index;
        return prev;
    }

    public IInternalContributor? UnsetContributor(int index)
    {
        IInternalContributor? prev = _layers[index];
        _layers[index] = null;

        if (index != _topIndex)
            return prev;

        for (var i = index - 1; i >= 0; i--)
        {
            if (_layers[i] is not null)
            {
                _topIndex = i;
                Active = _layers[i];
                return prev;
            }
        }

        _topIndex = -1;
        Active = null;

        return prev;
    }
}