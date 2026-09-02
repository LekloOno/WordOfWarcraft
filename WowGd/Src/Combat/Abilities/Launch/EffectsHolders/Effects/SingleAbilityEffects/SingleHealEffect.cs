using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public partial class SingleHealEffect : SingleAbilityEffect
{
    public override string Id => "effect_single_heal";
    [Export] private float _hp = 10f;

    public override void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight) =>
        Heal(entity, gatherWeight * actuateWeight);

    public override void LauncherEffect(IEntity entity, float actuateWeight) =>
        Heal(entity, actuateWeight);

    private void Heal(IEntity entity, float size) =>
        entity.Health.Heal(Mathf.FloorToInt(_hp * size));
}