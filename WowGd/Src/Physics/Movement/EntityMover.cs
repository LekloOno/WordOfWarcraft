using System;
using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.Channels;
using WowGd.Src.Physics.Movement.Channels.Additive;
using WowGd.Src.Physics.Movement.Channels.Internal;
using WowGd.Src.Physics.Movement.Status;

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

    private readonly IContributorChannel[] _contributorChannels =
        new IContributorChannel[Enum.GetValues<MovementChannels>().Length];

    public InternalChannel Internal => _internal;
    private readonly InternalChannel _internal = new();

    public EntityMover(IEntity entity)
    {
        _entity = entity;
        _statusChannels.Enabled  += OnEnabled;
        _statusChannels.Disabled += OnDisabled;

        for (int i = 0; i < Enum.GetValues<MovementChannels>().Length; i ++)
            _contributorChannels[i] = new AdditiveChannel();

        int internalIndex = System.Numerics.BitOperations.TrailingZeroCount((int) MovementChannels.Internal);

        _contributorChannels[internalIndex] = _internal;
        _internal.SetGrounded();
    }

    private void OnDisabled(MovementStatus status)
    {
        MovementChannels disabled = status.ToMovementChannels();

        MovementChannels &= ~disabled;

        if (_statusChannels.State.Airborne())
            _internal.SetAirborne();
        else
            _internal.SetGrounded();
    }

    private void OnEnabled(MovementStatus status)
    {
        MovementChannels enabled = status.ToMovementChannels();

        MovementChannels |= enabled;

        if (_statusChannels.State.Airborne())
            _internal.SetAirborne();
        else
            _internal.SetGrounded();
    }

    public void RunChannels(float delta)
    {
        float Friction = _internal.FrictionBase;

        uint bits = (uint)MovementChannels;
        Contribution contrib = new();

        while (bits != 0)
        {
            int index = System.Numerics.BitOperations.TrailingZeroCount(bits);
            contrib += _contributorChannels[index].GetContribution(this, delta); 
            bits &= bits - 1;
        }

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

        Vector2 currentWishDir = MovementChannels.HasFlag(MovementChannels.Internal) ?
            _internal.WishDir.WishDir() :
            Vector2.Zero;

		float currentSpeed = Body.Inertia.Dot(currentWishDir);
		Vector2 drag = -Friction * Body.Inertia;

		if (currentWishDir != Vector2.Zero && currentSpeed <= _internal.MaxSpeed)
		{
			float communeDrag = Mathf.Max(0, drag.Dot(-currentWishDir));
			drag += communeDrag * currentWishDir;
		}

		return drag;
    }

    public bool AddContributor(MovementChannels channel, IContributor contributor, uint priority, bool strict = false)
    {
        if (strict && !MovementChannels.HasFlag(channel))
            return false;

        int index = System.Numerics.BitOperations.TrailingZeroCount((uint)channel);
        
        if (index >= Enum.GetValues<MovementChannels>().Length)
            return false;

        return _contributorChannels[index].AddContributor(contributor, priority);
    }

    public bool RemoveContributor(MovementChannels channel, IContributor contributor)
    {
        int index = System.Numerics.BitOperations.TrailingZeroCount((uint)channel);
        
        if (index >= Enum.GetValues<MovementChannels>().Length)
            return false;

        return _contributorChannels[index].RemoveContributor(contributor);
    }

    private readonly Queue<(MovementChannels, IContributor)> _removeQueued = [];
    public void QueueRemoveContributor(MovementChannels channel, IContributor contributor) =>
        _removeQueued.Enqueue((channel, contributor));

    public void OnDied()
    {
        MovementChannels &= ~MovementChannels.Internal;
    }

    public void OnResurrected(int hp)
    {
        MovementChannels |= MovementChannels.Internal;
    }

    public void OnConsumed(int fp) { }
    public void OnGenerated(int fp) { }
}