using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting.Payload;

public static partial class TargetExt
{
    public static bool IsAlly(this Target target, bool includeSelf = false)
    {
        if (includeSelf)
            return !target.Relation.IsEnemy();
        return target.Relation.IsAlly();
    }

    public static bool IsEnemy(this Target target) =>
        target.Relation.IsEnemy();

    public static bool IsSelf(this Target target) =>
        target.Relation.IsSelf();

    public static TargetRelation GetRelationTo(this IEntity left, IEntity right)
    {
        if (right == left)
            return TargetRelation.Self;
        else if (right.TeamMask == left.TeamMask)
            return TargetRelation.Ally;
        
        return TargetRelation.Enemy;
    }

    public static bool TryInto(this TargetIntent intent, IEntity caster, out Target target, float gatherWeight = 1f)
    {
        if (intent.Entity is not IEntity entity)
        {
            target = default;
            return false;
        }

        target = new(entity, entity.GetRelationTo(caster), true, gatherWeight);
        return true;
    }
}