using Godot;
using WowGd.Src.Combat.Abilities.Exp.Targeting.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Effects.SingleAbilityEffects;


/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public abstract partial class SingleAbilityEffect : Effect
{
    [Export] private bool _launcher;
    [Export] private TargetRelation _target;

    public override bool Apply(TargetsPayload targetsPayload)
    {
        if (_launcher)
            LauncherEffect(targetsPayload.Launcher, targetsPayload.ActuateWeight);

        if (_target == TargetRelation.None)
            return true;

        targetsPayload.Apply(
            _target.HasSelf() ? Effect : null,
            _target.HasAlly() ? Effect : null,
            _target.HasEnemy() ? Effect : null
        );

        return true;
    }

    public abstract void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight);
    public abstract void LauncherEffect(IEntity entity, float actuateWeight);
}