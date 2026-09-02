using Godot;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WowGd.Src.Entities;

public static partial class EntitiesRegistry
{
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
                float squaredDist = (entity.Body.GlobalPosition - self.Body.GlobalPosition).LengthSquared();
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
        
        foreach (IEntity entity in Entities)
        {
            if (excludeSelf && entity == self)
                continue;

            if ((entity.TeamMask & targetTeamMask) != 0)
            {
                float squaredDist = (entity.Body.GlobalPosition - self.Body.GlobalPosition).LengthSquared();
                if (squaredDist <= rangeSquared)
                    inRange.Add(entity);
            }
        }

        return inRange;
    }

    public static HashSet<IEntity> GetEntitiesInRange(Vector2 position, float range)
    {
        HashSet<IEntity> inRange = [];
        float rangeSquared = range * range;
        
        foreach (IEntity entity in Entities)
        {
            float squaredDist = (entity.Body.GlobalPosition - position).LengthSquared();
            if (squaredDist <= rangeSquared)
                inRange.Add(entity);
        }

        return inRange;
    }

    public static float DistanceTo(this IEntity left, IEntity right) =>
        left.Body.GlobalPosition.DistanceTo(right.Body.GlobalPosition);

    public static float DistanceSquaredTo(this IEntity left, IEntity right) =>
        left.Body.GlobalPosition.DistanceSquaredTo(right.Body.GlobalPosition);
}