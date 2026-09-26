using System.Collections.Generic;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public class InternalChannel : IContributorChannel
{
    public readonly InternalContributor GroundInternal = new();
    public readonly InternalContributor AirInternal = new();
    private InternalContributor _currentInternal = null!;

    private readonly Dictionary<InternalLayer, InternalPriority> _registeredContribs = [];

    public float FrictionBase => _currentInternal.Current.FrictionBase;
    public float MaxSpeed => _currentInternal.Current.MaxSpeed;
    public IWishDir WishDir => _currentInternal.Current.WishDir;

    public Contribution GetContribution(EntityMover mover, float delta) =>
        _currentInternal.GetContribution(mover, delta);

    public void Close() =>
        _currentInternal.OnChannelClosed();

    public void Open() =>
        _currentInternal.OnChannelOpened();

    public void SetGrounded() =>
        _currentInternal = GroundInternal;

    public void SetAirborne() =>
        _currentInternal = AirInternal;

    public bool AddContributor(IContributor contributor, uint priority)
    {
        if (contributor is not InternalLayer contrib)
            return false;

        if (_registeredContribs.ContainsKey(contrib))
            return false;

        InternalPriority prio = (InternalPriority) priority;

        InternalContributor layers = prio.Grounded() ?
            GroundInternal : AirInternal;

        switch (prio.Layer())
        {
            // Base cannot be unset, only replaced, so no need to add it to registeredContribs.
            case InternalPriority.Base :
                layers.SetBase(contrib);
                return true;    // -- Skip

            case InternalPriority.Tackle :
                if (layers.SetTackle(contrib) is InternalLayer prevTackle)
                    _registeredContribs.Remove(prevTackle);
                break;

            case InternalPriority.Override :
                if (layers.SetOverride(contrib) is InternalLayer prevOverride)
                    _registeredContribs.Remove(prevOverride);
                break;

            default :
                return false;
        }

        _registeredContribs.Add(contrib, prio);
        return true;
    }

    public bool RemoveContributor(IContributor contributor)
    {
        if (contributor is not InternalLayer contrib)
            return false;

        if (!_registeredContribs.TryGetValue(contrib, out InternalPriority prio))
            return false;

        InternalContributor layers = prio.Grounded() ?
            GroundInternal : AirInternal;

        switch (prio.Layer())
        {
            case InternalPriority.Tackle :
                layers.UnsetTackle();
                break;

            case InternalPriority.Override :
                layers.UnsetOverride();
                break;

            // Base cannot be unset, it can only be replaced.
            // There should not be anyway a base can end up in the registeredContribs anyways.
            default :
                return false;
        }

        _registeredContribs.Remove(contrib);
        return true;
    }
}