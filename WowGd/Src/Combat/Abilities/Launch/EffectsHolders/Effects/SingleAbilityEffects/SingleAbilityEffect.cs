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

        targetsPayload.Apply(
            _target.HasSelf() ? ResolveEffect : null,
            _target.HasAlly() ? ResolveEffect : null,
            _target.HasEnemy() ? ResolveEffect : null
        );

        return true;
    }

    private void ResolveEffect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight) =>
        Effect(entity, IsDirect,
            _weighting.HasFlag(EffectWeighting.Gather)  ? gatherWeight  : 1f,
            _weighting.HasFlag(EffectWeighting.Actuate) ? actuateWeight : 1f);

    public abstract void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight);
    public abstract void LauncherEffect(IEntity entity, float actuateWeight);
}