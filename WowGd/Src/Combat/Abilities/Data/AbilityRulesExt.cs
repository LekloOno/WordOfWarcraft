using System.Collections.Generic;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.CasterRules;
using WowGd.Src.Combat.Abilities.LoopRules;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Abilities.Targeting.TargetRules;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Data;

public static class AbilityRulesExt
{
    public static bool CheckAll(this ICasterRule[] rules, IEntity caster)
    {
        foreach (ICasterRule rule in rules)
            if (!rule.Check(caster))
                return false;

        return true;
    }

    public static bool CheckAll(this IEnumerable<ICasterRule> rules, IEntity caster)
    {
        foreach (ICasterRule rule in rules)
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

    public static bool CheckAll(this ITargetRule[] rules, IEntity caster, TargetIntent intent)
    {
        foreach (ITargetRule rule in rules)
            if (!rule.Check(caster, intent))
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

    public static bool CheckAll(this Array<TargetRule> rules, IEntity caster, TargetIntent intent)
    {
        foreach (TargetRule rule in rules)
            if (!rule.Check(caster, intent))
                return false;

        return true;
    }

    public static bool CheckAll(this ILoopRule[] rules, ActuatePayload payload)
    {
        foreach (ILoopRule rule in rules)
            if (!rule.Check(payload))
                return false;

        return true;
    }

    public static bool CheckAll(this IEnumerable<ILoopRule> rules, ActuatePayload payload)
    {
        foreach (ILoopRule rule in rules)
            if (!rule.Check(payload))
                return false;

        return true;
    }

    public static bool CheckAll(this Array<LoopRule> rules, ActuatePayload payload)
    {
        foreach (LoopRule rule in rules)
            if (!rule.Check(payload))
                return false;

        return true;
    }
}