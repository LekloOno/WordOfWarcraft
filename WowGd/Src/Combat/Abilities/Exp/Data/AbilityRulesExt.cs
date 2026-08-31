using System.Collections.Generic;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.CasterRules;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Data;

public static class AbilityRulesExt
{
    public static bool CheckAll(this CasterRule[] rules, IEntity caster)
    {
        foreach (CasterRule rule in rules)
            if (!rule.Check(caster))
                return false;

        return true;
    }

    public static bool CheckAll(this ICollection<CasterRule> rules, IEntity caster)
    {
        foreach (CasterRule rule in rules)
            if (!rule.Check(caster))
                return false;

        return true;
    }

    public static bool CheckAll(this Array<CasterRule> rules, IEntity caster)
    {
        foreach (CasterRule rule in rules)
            if (!rule.Check(caster))
                return false;

        return true;
    }

    public static bool CheckAll(this TargetRule[] rules, IEntity caster, TargetIntent intent)
    {
        foreach (TargetRule rule in rules)
            if (!rule.Check(caster, intent))
                return false;

        return true;
    }

    public static bool CheckAll(this ICollection<TargetRule> rules, IEntity caster, TargetIntent intent)
    {
        foreach (TargetRule rule in rules)
            if (!rule.Check(caster, intent))
                return false;

        return true;
    }

    public static bool CheckAll(this Array<TargetRule> rules, IEntity caster, TargetIntent intent)
    {
        foreach (TargetRule rule in rules)
            if (!rule.Check(caster, intent))
                return false;

        return true;
    }
}