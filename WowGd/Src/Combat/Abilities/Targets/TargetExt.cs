using System.Collections.Generic;

namespace WowGd.Src.Combat.Abilities.Targets;

public static class TargetExt
{
    public static bool IsAlly(this Target target, bool includeSelf = false)
    {
        if (includeSelf)
            return target.Relation != TargetRelation.Enemy;
        return target.Relation == TargetRelation.Ally;
    }

    public static bool IsEnemy(this Target target) =>
        target.Relation == TargetRelation.Enemy;

    public static bool IsSelf(this Target target) =>
        target.Relation == TargetRelation.Self;

    public static IReadOnlySet<Target> GetAllies(this TargetsPayload payload, bool includeSelf = false)
    {
        HashSet<Target> allies = [];

        foreach(Target target in payload.Targets)
            if (target.IsAlly(includeSelf))
                allies.Add(target);

        return allies;
    }

    private static IReadOnlySet<Target> GetEnemies(this TargetsPayload payload)
    {
        HashSet<Target> allies = [];

        foreach(Target target in payload.Targets)
            if (target.IsEnemy())
                allies.Add(target);

        return allies;
    }

    private static IReadOnlySet<Target> Get(this TargetsPayload payload)
    {
        HashSet<Target> allies = [];

        foreach(Target target in payload.Targets)
            if (target.IsEnemy())
                allies.Add(target);

        return allies;
    }
}