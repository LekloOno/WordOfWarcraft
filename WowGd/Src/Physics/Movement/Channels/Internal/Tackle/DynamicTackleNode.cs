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
    private double _stamina;

    public event Action<StaminaChange>? StaminaChanged;

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

    private double _acc = 0f;
    public override void _PhysicsProcess(double delta)
    {
        _acc += delta;

        if (!PhysicsExt.IsProcessTick(0xf, 1))
            return;

        double decay = _acc / TackleStatisticsExt.GetTackleWeight(Entity, _tackleNode.Tackled!.Entity);
        _acc = 0f;

        if (ApplyStamina(StaminaChangeKind.Tick, -decay))
            return;
        
        if (!IsInRange())
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
        if (target.Health.Dead())
            return false;

        IEntityMover entityMover = Entity.EntityMover;

        if (!entityMover.StatusChannels.State.CanTackle())
            return false;

        IBody entityBody = Entity.Body;
        IBody targetBody = target.Body;

        float reachMeters = Entity.ReachMetres();
        if (!PhysicsExt.IsContactWithin(entityBody, targetBody, reachMeters))
            return false;

        TackleNode targetTackleNode = target.EntityMover.DynamicTackleNode._tackleNode;

        float limit = Entity.TackleSpeedMs();
        if (!_tackleNode.Tackle(targetTackleNode, limit))
            return false;

        Offset = entityBody.GlobalPosition - targetBody.GlobalPosition;
        entityMover.Internal.StartOverride(new TackleWishDir(Entity, target, Offset));

        _stamina = 1f;
        _acc = 0f;

        SetPhysicsProcess(true);
        return true;
    }

    public bool ReleaseTackle() => _tackleNode.Release();
    public int DodgeTacklers() => _tackleNode.DodgeTacklers();
    
    public void DrainTacklers(double amount)
    {
        var tacklers = _tackleNode.Tacklers;
        for (int i = tacklers.Count - 1; i >= 0; i--)
        {
            if (i >= tacklers.Count) continue;
            tacklers[i].Entity.EntityMover.DynamicTackleNode.DrainStamina(amount);
        }
    }

    public void DrainTacklersWeighted(double weight)
    {
        var tacklers = _tackleNode.Tacklers;
        for (int i = tacklers.Count - 1; i >= 0; i--)
        {
            if (i >= tacklers.Count) continue;

            double amount = TackleStatisticsExt.GetTackleDrain(tacklers[i].Entity, Entity, weight);
            tacklers[i].Entity.EntityMover.DynamicTackleNode.DrainStamina(amount);
        }
    }

    public void DrainStamina(double amount)
    {
        if (IsTackling)
            ApplyStamina(StaminaChangeKind.Drain, -Math.Max(amount, 0));
    }

    public void FeedStamina(double amount)
    {
        if (IsTackling)
            ApplyStamina(StaminaChangeKind.Feed, Math.Max(amount, 0));
    }

    private bool ApplyStamina(StaminaChangeKind kind, double requested)
    {
        double prev = _stamina;
        _stamina = Math.Clamp(prev + requested, 0.0, 1.0);
        double delta = _stamina - prev;

        if (delta == 0 && kind != StaminaChangeKind.Tick)
            return false;

        StaminaChanged?.Invoke(new(kind, delta, _stamina));

        if (_stamina <= 0)
            return ReleaseTackle();
        return false;
    }
}