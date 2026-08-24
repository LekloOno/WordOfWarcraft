using System.Collections.Generic;

namespace WowGd.Src.Entities;

public static class EntitiesRegistry
{
    private static readonly HashSet<IEntity> _entities = [];
    public static IReadOnlySet<IEntity> Entities => _entities;

    public static bool Register(IEntity entity) =>
        _entities.Add(entity);

    public static bool Unregister(IEntity entity) =>
        _entities.Remove(entity);
}