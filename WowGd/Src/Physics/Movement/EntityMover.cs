using System;
using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.Channels;
using WowGd.Src.Physics.Movement.Internal;
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

    private readonly List<IMovementContributor>[] _contributorChannels =
        new List<IMovementContributor>[Enum.GetValues<MovementChannels>().Length];

    public InternalMovement Internal => _internal;
    private readonly InternalMovement _internal = new();

    public EntityMover(IEntity entity)
    {
        _entity = entity;
        _statusChannels.Enabled  += OnEnabled;
        _statusChannels.Disabled += OnDisabled;

        for (int i = 0; i < Enum.GetValues<MovementChannels>().Length; i ++)
            _contributorChannels[i] = [];

        int internalIndex = System.Numerics.BitOperations.TrailingZeroCount((int) MovementChannels.Internal);

        _contributorChannels[internalIndex] = [_internal];
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

        Vector2 forces = Vector2.Zero;
        float frictionRatio = 1f;

        while (bits != 0)
        {
            int index = System.Numerics.BitOperations.TrailingZeroCount(bits);
            
            foreach (IMovementContributor contributor in _contributorChannels[index])
            {
                contributor.GetContribution(this, delta, out Vector2 force, out float friction);
                forces += force;
                frictionRatio *= friction;
            }

            bits &= bits - 1;
        }

        while (_removeQueued.TryDequeue(out (MovementChannels, IMovementContributor) queued))
            RemoveContributor(queued.Item1, queued.Item2);

        Body.SetVelocity(forces);
        Vector2 drag = GetDrag(Friction * frictionRatio);
        Body.SetVelocity(Body.LinearVelocity + drag * delta);
        //Body.ApplyForce(drag);
    }

    private Vector2 GetDrag(float Friction)
    {
        if (Friction == 0f)
            return Vector2.Zero;

        Vector2 currentWishDir = MovementChannels.HasFlag(MovementChannels.Internal) ?
            _internal.WishDir.WishDir() :
            Vector2.Zero;

		float currentSpeed = Body.LinearVelocity.Dot(currentWishDir);
		Vector2 drag = -Friction * Body.LinearVelocity;

		if (currentWishDir != Vector2.Zero && currentSpeed <= _internal.MaxSpeed)
		{
			float communeDrag = Mathf.Max(0, drag.Dot(-currentWishDir));
			drag += communeDrag * currentWishDir;
		}

		return drag;
    }

    public bool AddContributor(MovementChannels channel, IMovementContributor contributor, bool strict = false)
    {
        if (strict && !MovementChannels.HasFlag(channel))
            return false;

        int index = System.Numerics.BitOperations.TrailingZeroCount((uint)channel);
        
        if (index >= Enum.GetValues<MovementChannels>().Length)
            return false;

        _contributorChannels[index].Add(contributor);
        return true;
    }

    public bool RemoveContributor(MovementChannels channel, IMovementContributor contributor)
    {
        int index = System.Numerics.BitOperations.TrailingZeroCount((uint)channel);
        
        if (index >= Enum.GetValues<MovementChannels>().Length)
            return false;

        return _contributorChannels[index].Remove(contributor);
    }

    private readonly Queue<(MovementChannels, IMovementContributor)> _removeQueued = [];
    public void QueueRemoveContributor(MovementChannels channel, IMovementContributor contributor) =>
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