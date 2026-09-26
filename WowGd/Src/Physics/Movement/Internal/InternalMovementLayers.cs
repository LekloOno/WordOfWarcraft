using Godot;
using WowGd.Src.Physics.Movement.Channels;

namespace WowGd.Src.Physics.Movement.Internal;

public class InternalMovementLayers : IMovementContributor
{
    private InternalMovementLayer? _override;
    private InternalMovementLayer? _tackle;
    private InternalMovementLayer _base = null!;

    public InternalMovementLayer Current { get; private set; } = null!;

    public void SetOverride(InternalMovementLayer @override)
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

    public void SetTackle(InternalMovementLayer tackle)
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

    public void SetBase(InternalMovementLayer @base)
    {
        if (Current == _base)
            Current = @base;
            
        _base = @base;
    }

    public void GetContribution(EntityMover mover, float delta, out Vector2 force, out float frictionRatio) =>
        Current.GetContribution(mover, delta, out force, out frictionRatio);

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