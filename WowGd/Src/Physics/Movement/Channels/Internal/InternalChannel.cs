using System;
using System.Collections.Generic;
using Godot;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public class InternalChannel : IContributorChannel
{
    public readonly IInternalLayer GroundInternal;
    public readonly IInternalLayer AirInternal;
    private IInternalLayer _currentInternal;

    private readonly Dictionary<IInternalContributor, InternalPriority> _registeredContribs = [];

    public float FrictionBase => _currentInternal.Active?.FrictionBase ?? 0;
    public float MaxSpeed => _currentInternal.Active?.MaxSpeed ?? 0;

    private IWishDir _baseWishDir;
    public IWishDir BaseWishDir
    {
        get => _baseWishDir;
        set
        {
            if (_baseWishDir == value)
                return;

            if (GetWishDir == _baseWishDir.WishDir)
                GetWishDir = value.WishDir;

            _baseWishDir = value;
        }
    }

    private Func<Vector2> GetWishDir;
    public Vector2 WishDir => GetWishDir();

    public InternalChannel(IWishDir baseWishDir, bool startGrounded = true)
        : this(new InternalLayer(), new InternalLayer(), baseWishDir, startGrounded) {}

    public InternalChannel(
        IInternalContributor ground,
        IInternalContributor air,
        IWishDir baseWishDir,
        bool startGrounded = true)
        : this(new InternalLayer(ground), new InternalLayer(air), baseWishDir, startGrounded) {}

    private InternalChannel(IInternalLayer ground, IInternalLayer air, IWishDir baseWishDir, bool startGrounded)
    {
        GroundInternal  = ground;
        AirInternal     = air;
        
        _currentInternal = startGrounded ? GroundInternal : AirInternal;

        _baseWishDir = baseWishDir;
        GetWishDir  = baseWishDir.WishDir;
    }

    public Contribution GetContribution(EntityMover mover, float delta) =>
        _currentInternal.GetContribution(WishDir, mover, delta);

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
        if (contributor is not IInternalContributor contrib)
            return false;

        if (_registeredContribs.ContainsKey(contrib))
            return false;

        InternalPriority prio = (InternalPriority) priority;

        IInternalLayer layer = prio.Grounded() ?
            GroundInternal : AirInternal;

        layer.SetContributor(prio.PriorityIndex(), contrib);

        _registeredContribs.Add(contrib, prio);
        return true;
    }

    public bool RemoveContributor(IContributor contributor)
    {
        if (contributor is not IInternalContributor contrib)
            return false;

        if (!_registeredContribs.TryGetValue(contrib, out InternalPriority prio))
            return false;

        IInternalLayer layer = prio.Grounded() ?
            GroundInternal : AirInternal;

        layer.UnsetContributor(prio.PriorityIndex());

        _registeredContribs.Remove(contrib);
        return true;
    }

    public void StartOverride(IWishDir tackleWishDir) =>
        GetWishDir = tackleWishDir.WishDir;

    public void ReleaseOverride()
    {
        GetWishDir = BaseWishDir.WishDir;
    }
}