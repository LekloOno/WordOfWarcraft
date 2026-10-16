using System;
using System.Collections.Generic;
using CombatCore;
using CombatCore.Primitives;
using WowGd.Src.Combat;

namespace WowGd.Src.Entities;

public partial class EntitiesRegistry : IEntityDirectory
{
    private readonly HashSet<IEntity> _entities = [];
    private readonly Dictionary<EntityId, IEntity> _orderedEntities = [];
    public IReadOnlySet<IEntity> Entities => _entities;
    public event Action<IEntity>? EntityCreated;
    public event Action<IEntity>? EntityDeleted;
    private uint _count = 0;

    public bool Register(IEntity entity)
    {
        bool created = _entities.Add(entity);
        if (!created)
            return false;

        entity.SetId(new(_count++));
        _orderedEntities.Add(entity.Id, entity);
        EntityCreated?.Invoke(entity);
        return true;
    }

    public bool Unregister(IEntity entity)
    {
        bool deleted = _entities.Remove(entity);
        if (deleted)
            EntityDeleted?.Invoke(entity);

        _orderedEntities.Remove(entity.Id);
        CombatCoreNode.Authority.RemoveEntity(entity.Id);
        
        return deleted;
    }

    public ICombatEntity? Find(EntityId id) => FindAsEntity(id);

    public IEntity? FindAsEntity(EntityId id)
    {
        _orderedEntities.TryGetValue(id, out IEntity? entity);
        return entity;
    }
}