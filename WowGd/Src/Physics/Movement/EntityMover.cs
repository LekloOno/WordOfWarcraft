using System;
using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.Channels;
using WowGd.Src.Physics.Movement.Channels.Additive;
using WowGd.Src.Physics.Movement.Channels.Internal;
using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;
using WowGd.Src.Physics.Movement.Status;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

public class EntityMover : IEntityMover, IEntityHealthHandler
{
    private readonly IEntity _entity;
	public IBody Body => _entity.Body;
    
    public MovementChannels MovementChannels { get; private set; } =
        MovementChannels.God |
        MovementChannels.Internal |
        MovementChannels.External |
        MovementChannels.Warp;

    public StatusQueryRegister StatusChannels => _statusChannels;
    private readonly StatusQueryRegister _statusChannels = new();

    private readonly int _channels = Enum.GetValues<MovementChannels>().Length;
    private readonly IContributorChannel[] _contributorChannels =
        new IContributorChannel[Enum.GetValues<MovementChannels>().Length];

    public InternalChannel Internal { get; }
    public DynamicTackleNode DynamicTackleNode { get; }

    public EntityMover(IEntity entity)
    {
        _entity = entity;
        _statusChannels.Enabled  += OnStatusEnabled;
        _statusChannels.Disabled += OnStatusDisabled;

        DynamicTackleNode = new(entity);

        for (int i = 0; i < Enum.GetValues<MovementChannels>().Length; i ++)
            _contributorChannels[i] = new AdditiveChannel();

        int internalIndex = System.Numerics.BitOperations.TrailingZeroCount((int) MovementChannels.Internal);

        Internal = new(entity.WishDir, true);
        _contributorChannels[internalIndex] = Internal;
        Internal.SetGrounded();
    }

    private void OnStatusDisabled(MovementStatus status)
    {
        MovementChannels opened = status.ToMovementChannels();

        foreach (int index in BitFlags.Enumerate((uint)opened))
            _contributorChannels[index].Open();

        MovementChannels |= opened;
        UpdateInternal();
    }

    private void OnStatusEnabled(MovementStatus status)
    {
        MovementChannels closed = status.ToMovementChannels();

        foreach (int index in BitFlags.Enumerate((uint)closed))
            _contributorChannels[index].Close();

        if (DynamicTackleNode.IsTackling && !status.CanTackle())
            DynamicTackleNode.ReleaseTackle();

        MovementChannels &= ~closed;
        UpdateInternal();
    }

    private void UpdateInternal()
    {
        if (_statusChannels.State.Airborne())
            Internal.SetAirborne();
        else
            Internal.SetGrounded();
    }

    public void RunChannels(float delta)
    {
        float Friction = Internal.FrictionBase;

        Contribution contrib = new();

        foreach (int index in BitFlags.Enumerate((uint)MovementChannels))
            contrib += _contributorChannels[index].GetContribution(this, delta);

        while (_removeQueued.TryDequeue(out (MovementChannels, IContributor) queued))
            RemoveContributor(queued.Item1, queued.Item2);

        Body.AddRawForce(contrib.RawVelocity);
        Body.Accelerate(contrib.Acceleration);
        Vector2 drag = GetDrag(Friction * contrib.FrictionRatio);
        Body.Accelerate(drag * delta);
    }

    private Vector2 GetDrag(float Friction)
    {
        if (Friction == 0f)
            return Vector2.Zero;

		Vector2 drag = -Friction * Body.Inertia;

        Vector2 currentWishDir = MovementChannels.HasFlag(MovementChannels.Internal) ?
            Internal.WishDir : Vector2.Zero;

		float currentSpeed = Body.Inertia.Dot(currentWishDir);

		if (currentWishDir != Vector2.Zero && currentSpeed <= Internal.MaxSpeed)
		{
			float communeDrag = Mathf.Max(0, drag.Dot(-currentWishDir));
			drag += communeDrag * currentWishDir;
		}

		return drag;
    }

    public ChannelSubResult AddContributor(MovementChannels channel, IContributor contributor, uint priority, bool strict = false)
    {
        int index = System.Numerics.BitOperations.TrailingZeroCount((uint)channel);
        
        if (index >= _channels)
            return ChannelSubResult.None;

        bool opened = MovementChannels.HasFlag(channel);

        if (strict && !opened)
            return ChannelSubResult.None;

        ChannelSubResult result = opened
            ? ChannelSubResult.ChannelOpened
            : ChannelSubResult.None;

        if (_contributorChannels[index].AddContributor(contributor, priority))
            result |= ChannelSubResult.Success;

        return result;
    }

    public ChannelSubResult RemoveContributor(MovementChannels channel, IContributor contributor)
    {
        int index = System.Numerics.BitOperations.TrailingZeroCount((uint)channel);
        
        if (index >= _channels)
            return ChannelSubResult.None;

        ChannelSubResult result = MovementChannels.HasFlag(channel)
            ? ChannelSubResult.ChannelOpened
            : ChannelSubResult.None;

        if (_contributorChannels[index].RemoveContributor(contributor))
            result |= ChannelSubResult.Success;

        return result;
    }

    private readonly Queue<(MovementChannels, IContributor)> _removeQueued = [];
    public void QueueRemoveContributor(MovementChannels channel, IContributor contributor) =>
        _removeQueued.Enqueue((channel, contributor));

    public void OnDied()
    {
        StatusChannels.Query(this, MovementStatus.Immobilized | MovementStatus.Anchored, out _);
    }

    public void OnResurrected(int hp)
    {
        StatusChannels.Unquery(this);
    }

    public void OnConsumed(int fp) { }
    public void OnGenerated(int fp) { }
}