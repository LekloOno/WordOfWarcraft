using CombatCore.Sources;
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

    public override bool Apply(TargetsPayload targetsPayload, Source source, Source? cause = null)
    {
        if (_launcher)
            LauncherEffect(targetsPayload, source, targetsPayload.ActuateWeight, cause);

        if (_target == TargetRelation.None)
            return true;

        void solver(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight) =>
            ResolveEffect(targetsPayload.Launcher, entity, source, IsDirect, gatherWeight, actuateWeight, cause);

        targetsPayload.Apply(
            _target.HasSelf()  ? solver : null,
            _target.HasAlly()  ? solver : null,
            _target.HasEnemy() ? solver : null
        );

        return true;
    }

    private void ResolveEffect(IEntity launcher, IEntity target, Source source, bool IsDirect, float gatherWeight, float actuateWeight, Source? cause) =>
        Effect(launcher, target, source, IsDirect,
            _weighting.HasFlag(EffectWeighting.Gather)  ? gatherWeight  : 1f,
            _weighting.HasFlag(EffectWeighting.Actuate) ? actuateWeight : 1f, cause);

    protected abstract void Effect(
        IEntity launcher,
        IEntity target,
        Source source,
        bool IsDirect,
        float gatherWeight,
        float actuateWeight,
        Source? cause);

    protected virtual void LauncherEffect(TargetsPayload targetsPayload, Source source, float actuateWeight, Source? cause) =>
        Effect(
            launcher: targetsPayload.Launcher,
            target: targetsPayload.Launcher,
            source: source,
            IsDirect: true,
            gatherWeight: 1f,
            actuateWeight,
            cause: cause
        );
}