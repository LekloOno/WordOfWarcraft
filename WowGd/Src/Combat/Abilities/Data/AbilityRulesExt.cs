using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Data;

public static class AbilityRulesExt
{
    public static bool CheckAll(this IEnumerable<ICasterRule> rules, IEntity caster)
    {
        foreach (ICasterRule rule in rules)
            if (!rule.Check(caster))
                return false;

        return true;
    }

    public static bool CheckAll(this IEnumerable<ITargetRule> rules, IEntity caster, TargetIntent intent)
    {
        foreach (ITargetRule rule in rules)
            if (!rule.Check(caster, intent))
                return false;

        return true;
    }

    public static bool CheckAll(this IEnumerable<ILoopRule> rules, ActuatePayload payload)
    {
        bool valid = false;

        foreach (ILoopRule rule in rules)
        {
            valid = true;       // Cheaper way to check if enumerable is empty.
            if (!rule.Check(payload))
                return false;
        }

        return valid;
    }
}