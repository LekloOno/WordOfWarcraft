using System;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;
using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Entities;

[Flags]
public enum TeamMask
{
    Environment = 1 << 0,
    Team1       = 1 << 1,
    Team2       = 1 << 2,
}

public static class TeamMaskExt
{
    public static uint ToCollisionLayer(this TeamMask teamMask) =>
        (uint) teamMask;

    public static uint ToCollisionMask(this TeamMask teamMask) =>
        uint.MaxValue;

    public static uint ToAllyCollisionMask(this IEntity entity) =>
        entity.TeamMask.ToCollisionLayer();

    public static uint ToEnemyCollisionMask(this IEntity entity) =>
        (entity.TeamMask & ~TeamMask.Environment).ToCollisionLayer();

    public static uint ToCollisionMask(this IEntity entity, TargetRelation relation)
    {
        return relation switch
        {
            TargetRelation.None =>
                0,
            TargetRelation.Self or TargetRelation.Ally =>
                entity.ToAllyCollisionMask(),
            TargetRelation.Enemy =>
                entity.ToEnemyCollisionMask(),
            _ => throw new IndexOutOfRangeException(),
        };
    }

    public static uint ToCollisionMask(this IEntity entity, RadiusGathererSettings settings)
    {
        uint collisionMask = 0;
        
        if (settings.HasFlag(RadiusGathererSettings.WallsObstruction))
            collisionMask |= TeamMask.Environment.ToCollisionMask();

        if (settings.HasFlag(RadiusGathererSettings.AlliesObstruction))
            collisionMask |= entity.ToAllyCollisionMask();
            
        if (settings.HasFlag(RadiusGathererSettings.EnemiesObstruction))
            collisionMask |= entity.ToEnemyCollisionMask();

        return collisionMask;
    }
}