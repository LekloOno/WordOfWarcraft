using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Entities;
using WowGd.Src.Physics;

namespace WowGd.Src.Combat.Abilities.Mage;

public partial class Breach : Area2D
{
    public readonly BreachType Type;
    private readonly IBreachBuff _breachBuff = null!;

    private IEntity? _target;

    private readonly List<IEntity> _queuedEntities = [];
    private readonly CollisionShape2D[] _collisions;
    private readonly IEntity _owner;

    public Breach() {}

    public Breach(CollisionShape2D[] collisions, IBreachBuff breachBuff, IEntity owner)
    {
        _breachBuff = breachBuff;
        _collisions = collisions;
        _owner = owner;

        IdData = new($"quantum_mage.breach.{breachBuff.Key}");
        TeamMask = owner.TeamMask;

        foreach (var collision in collisions)
            AddChild(collision);

        BodyShapeEntered += OnBodyShapeEntered;
        BodyShapeExited += OnBodyShapeExited;
    }

    public override async void _Ready()
    {
        await EntitiesRegistryNode.Instance.Initialization;
        EntitiesRegistryNode.Registry.Register(this);
    }

    private void OnBodyShapeEntered(Rid bodyRid, Node2D body, long bodyShapeIndex, long localShapeIndex)
    {
        if (body is not IBody entityBody)
            return;

        IEntity entity = entityBody.Entity;
        _queuedEntities.Add(entity);
        
        if (_target != null)
            return;
            
        _target = entity;
        _breachBuff.Apply(_target, _target.GetRelationTo(_owner));
    }

    private void OnBodyShapeExited(Rid bodyRid, Node2D body, long bodyShapeIndex, long localShapeIndex)
    {
        if (body is not IBody entityBody)
            return;

        IEntity entity = entityBody.Entity;
        _queuedEntities.Remove(entity);

        if (entity != _target)
            return;

        _breachBuff.Cancel(_target, _target.GetRelationTo(_owner));

        if (_queuedEntities.Count == 0)
            return;

        _target = _queuedEntities[0];
        _breachBuff.Apply(_target, _target.GetRelationTo(_owner));
    }

    public void Release()
    {
        BodyShapeEntered -= OnBodyShapeEntered;
        BodyShapeExited -= OnBodyShapeExited;

        if (_target is not null)
            _breachBuff.Cancel(_target, _target.GetRelationTo(_owner));

        QueueFree();
    }
}