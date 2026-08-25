using System;
using System.Collections.Generic;

namespace WowGd.Src.Entities;

public static partial class EntitiesRegistry
{
    private static readonly HashSet<IEntity> _entities = [];
    public static IReadOnlySet<IEntity> Entities => _entities;
    public static event Action<IEntity>? EntityCreated;
    public static event Action<IEntity>? EntityDeleted;

    public static bool Register(IEntity entity)
    {
        bool created = _entities.Add(entity);
        if (created)
            EntityCreated?.Invoke(entity);
        return created;
    }

    public static bool Unregister(IEntity entity)
    {
        bool deleted = _entities.Remove(entity);
        if (deleted)
            EntityDeleted?.Invoke(entity);
        return deleted;
    }
}