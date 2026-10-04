using System;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Stats.Reach;
using WowGd.Src.Entities.Stats.Tackle;
using WowGd.Src.Physics.Movement.Status;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public partial class DynamicTackleNode : Node
{
    public const float TackleMargin = 0.6f;

    public readonly IEntity Entity;
    private readonly TackleNode _tackleNode;
    private Vector2 _offset;
    private float _maxRangeSquared;
    private float _stamina;

    public event Action<float, float>? StaminaTicked;

    public Vector2 Offset
    {
        get => _offset;
        set
        {
            if (_offset == value)
                return;

            _offset = value;
            float maxRange = _offset.Length() + TackleMargin;
            _maxRangeSquared = maxRange * maxRange;
        }
    }

    // For godot binding only, should not be used.
    public DynamicTackleNode()
    {
        Entity = null!;
        _tackleNode = null!;
        SetPhysicsProcess(false);
    }

    public DynamicTackleNode(IEntity entity) : this()
    {
        Entity = entity;
        _tackleNode = new(entity);

        _tackleNode.TackleStarted   += OnTackleStarted;
        _tackleNode.TackleReleased  += OnTackleReleased;
        _tackleNode.GotTackled      += OnGotTackled;
        _tackleNode.GotReleased     += OnGotReleased;

        if (Entity is Node node)
            node.AddChild(this);
            
        SetPhysicsProcess(false);
    }

    private float _acc = 0f;
    public override void _PhysicsProcess(double delta)
    {
        _acc += (float) delta;

        if (!PhysicsExt.IsProcessTick(0xf, 1))
            return;

        float prevStamina = _stamina;
        _stamina -= _acc / TackleStatisticsExt.GetTackleWeight(Entity, _tackleNode.Tackled!.Entity);
        _acc = 0f;

        StaminaTicked?.Invoke(prevStamina, _stamina);

        if (_stamina <= 0f || !IsInRange())
            ReleaseTackle();
    }

    private bool IsInRange()
    {
        if (_tackleNode.Tackled is null)
            return false;

        return Entity.DistanceSquaredTo(_tackleNode.Tackled.Entity) <= _maxRangeSquared;
    }

    public bool StartTackle(IEntity target)
    {
        IEntityMover entityMover = Entity.EntityMover;

        if (!entityMover.StatusChannels.State.CanTackle())
            return false;

        IBody entityBody = Entity.Body;
        IBody targetBody = target.Body;

        float reachMeters = Entity.GetMetersReach();
        if (!PhysicsExt.IsContactWithin(entityBody, targetBody, reachMeters))
            return false;

        TackleNode targetTackleNode = target.EntityMover.DynamicTackleNode._tackleNode;

        float limit = TackleStatisticsExt.GetMsTackle(Entity);
        if (!_tackleNode.Tackle(targetTackleNode, limit))
            return false;

        Offset = entityBody.GlobalPosition - targetBody.GlobalPosition;
        entityMover.Internal.StartOverride(new TackleWishDir(Entity, target, Offset));

        _stamina = 1f;
        _acc = 0f;

        SetPhysicsProcess(true);
        return true;
    }

    public bool ReleaseTackle()
    {
        if (!_tackleNode.Release())
            return false;
            
        Entity.EntityMover.Internal.ReleaseOverride();
        SetPhysicsProcess(false);
        return true;
    }
}