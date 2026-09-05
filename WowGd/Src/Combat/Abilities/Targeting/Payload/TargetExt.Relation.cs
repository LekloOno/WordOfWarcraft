using WowGd.Src.Entities;
using WowGd.Src.Render;

namespace WowGd.Src.Combat.Abilities.Targeting.Payload;

public static partial class TargetExt
{
    public static bool IsSelf(this TargetRelation relation) =>
        relation == TargetRelation.Self;

    public static bool IsAlly(this TargetRelation relation) =>
        relation == TargetRelation.Ally;

    public static bool IsEnemy(this TargetRelation relation) =>
        relation == TargetRelation.Enemy;

    public static bool HasSelf(this TargetRelation relation) =>
        relation.HasFlag(TargetRelation.Self);

    public static bool HasAlly(this TargetRelation relation) =>
        relation.HasFlag(TargetRelation.Ally);

    public static bool HasEnemy(this TargetRelation relation) =>
        relation.HasFlag(TargetRelation.Enemy);

    public static TargetRelation GetClientRelation(this IEntity entity)
    {
        if (entity == ClientInfo.Entity)
            return TargetRelation.Self;

        if (entity.TeamMask == ClientInfo.Entity.TeamMask)
            return TargetRelation.Ally;

        return TargetRelation.Enemy;
    }
}