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
    private Vector2 _offset;
    private float _maxRangeSquared;

    /// <summary>
    /// Occurs when this node starts tackling another node.
    /// </summary>
    public event Action<DynamicTackleNode>? TackleStarted;
    /// <summary>
    /// Occurs when this node stops tackling another node.
    /// </summary>
    public event Action<DynamicTackleNode>? TackleReleased;

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

    private void OnTackleStarted(TackleNode node) =>
        TackleStarted?.Invoke(node.Entity.EntityMover.DynamicTackleNode);

    private void OnTackleReleased(TackleNode node)
    {
        Entity.EntityMover.Internal.ReleaseOverride();
        TackleReleased?.Invoke(node.Entity.EntityMover.DynamicTackleNode);
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

        DynamicTackleNode targetDynamic = target.EntityMover.DynamicTackleNode;
        TackleNode targetTackleNode = targetDynamic._tackleNode;

        float limit = Entity.TackleSpeedMs();

        if (!targetDynamic.CanGetTackled())
            return false;

        if (!_tackleNode.Tackle(targetTackleNode, limit))
            return false;

        Offset = entityBody.GlobalPosition - targetBody.GlobalPosition;
        entityMover.Internal.StartOverride(new TackleWishDir(Entity, target, Offset));

        return true;
    }

    public bool ReleaseTackle() => _tackleNode.Release();
}