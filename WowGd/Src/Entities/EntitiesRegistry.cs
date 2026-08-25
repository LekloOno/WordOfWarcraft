using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WowGd.Src.Entities;

public static class EntitiesRegistry
{
    private static readonly HashSet<IEntity> _entities = [];
    public static IReadOnlySet<IEntity> Entities => _entities;

    public static bool Register(IEntity entity) =>
        _entities.Add(entity);

    public static bool Unregister(IEntity entity) =>
        _entities.Remove(entity);


    public static bool TryGetClosestTarget(this IEntity self, [NotNullWhen(true)] out IEntity? target, uint targetTeamMask = uint.MaxValue, bool excludeSelf = true)
    {
        target = null;
        float closestSquaredDist = float.PositiveInfinity;

        foreach (IEntity entity in _entities)
        {
            if (excludeSelf && entity == self)
                continue;

            if ((entity.TeamMask & targetTeamMask) != 0)
            {
                float squaredDist = (entity.Body.Position - self.Body.Position).LengthSquared();
                if (squaredDist < closestSquaredDist)
                {
                    target = entity;
                    closestSquaredDist = squaredDist;
                }
            }
        }

        return target != null;
    }

    public static bool TryGetTarget(this IEntity self, [NotNullWhen(true)] out IEntity? target, uint targetTeamMask = uint.MaxValue, bool excludeSelf = true)
    {
        foreach (IEntity entity in EntitiesRegistry.Entities)
        {
            if (excludeSelf && entity == self)
                continue;

            if ((entity.TeamMask & targetTeamMask) != 0)
            {
                target = entity;
                return true;
            }
        }

        target = null;
        return false;
    }

    public static ICollection<IEntity> GetTargetsInRange(this IEntity self, float range, uint targetTeamMask = uint.MaxValue, bool excludeSelf = true)
    {
        ICollection<IEntity> inRange = [];
        float rangeSquared = range * range;
        
        foreach (IEntity entity in EntitiesRegistry.Entities)
        {
            if (excludeSelf && entity == self)
                continue;

            if ((entity.TeamMask & targetTeamMask) != 0)
            {
                float squaredDist = (entity.Body.Position - self.Body.Position).LengthSquared();
                if (squaredDist <= rangeSquared)
                    inRange.Add(entity);
            }
        }

        return inRange;
    }
}