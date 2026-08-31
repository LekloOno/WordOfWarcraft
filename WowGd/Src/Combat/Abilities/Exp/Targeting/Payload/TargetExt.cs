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
}