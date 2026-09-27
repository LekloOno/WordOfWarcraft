using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;


/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public abstract partial class SingleAbilityEffect : Effect
{
    [Export] private bool _launcher;
    [Export] private TargetRelation _target;
    [Export] private EffectWeighting _weighting = EffectWeighting.Actuate | EffectWeighting.Gather;

    public override bool Apply(TargetsPayload targetsPayload)
    {
        if (_launcher)
            LauncherEffect(targetsPayload.Launcher, targetsPayload.ActuateWeight);

        if (_target == TargetRelation.None)
            return true;

        void solver(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight) =>
            ResolveEffect(targetsPayload.Launcher, entity, IsDirect, gatherWeight, actuateWeight);

        targetsPayload.Apply(
            _target.HasSelf()  ? solver : null,
            _target.HasAlly()  ? solver : null,
            _target.HasEnemy() ? solver : null
        );

        return true;
    }

    private void ResolveEffect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight) =>
        Effect(launcher, target, IsDirect,
            _weighting.HasFlag(EffectWeighting.Gather)  ? gatherWeight  : 1f,
            _weighting.HasFlag(EffectWeighting.Actuate) ? actuateWeight : 1f);

    public abstract void Effect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight);

    public virtual void LauncherEffect(IEntity entity, float actuateWeight) =>
        Effect(
            launcher: entity,
            target: entity,
            IsDirect: true,
            gatherWeight: 1f,
            actuateWeight
        );
}