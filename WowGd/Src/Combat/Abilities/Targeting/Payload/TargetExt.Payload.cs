using System;
using System.Collections.Generic;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting.Payload;

public static partial class TargetExt
{
    public static IReadOnlySet<Target> GetAllies(this TargetsPayload payload, bool includeSelf = false)
    {
        HashSet<Target> allies = [];

        foreach(Target target in payload.Targets)
            if (target.IsAlly(includeSelf))
                allies.Add(target);

        return allies;
    }

    public static IReadOnlySet<Target> GetEnemies(this TargetsPayload payload)
    {
        HashSet<Target> allies = [];

        foreach(Target target in payload.Targets)
            if (target.IsEnemy())
                allies.Add(target);

        return allies;
    }

    /// <summary>
    /// A single pass effect applier for happy comp-comp-comp-youuuutor
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="selfEffect"></param>
    /// <param name="allyEffect"></param>
    /// <param name="enemyEffect"></param>
    public static void Apply(
        this TargetsPayload payload,
        Action<IEntity, bool, float, float>? selfEffect   = null,
        Action<IEntity, bool, float, float>? allyEffect   = null,
        Action<IEntity, bool, float, float>? enemyEffect  = null)
    {
        foreach (Target target in payload.Targets)
            target.Relation.GetEffect(selfEffect, allyEffect, enemyEffect)(target.Entity, target.IsDirect, target.GatherWeight, payload.ActuateWeight);
    }

    private static Action<IEntity, bool, float, float> GetEffect(
        this TargetRelation relation,
        Action<IEntity, bool, float, float>? selfEffect,
        Action<IEntity, bool, float, float>? allyEffect,
        Action<IEntity, bool, float, float>? enemyEffect)
    {
        return relation switch
        {
            TargetRelation.Self => selfEffect ?? Pit,
            TargetRelation.Ally => allyEffect ?? Pit,
            TargetRelation.Enemy => enemyEffect ?? Pit,
            _ => Pit,
        };
    }

    private static void Pit(IEntity entity, bool isDirect, float gatherWeight, float actuateWeight) {}
}