using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public partial class SingleDamageEffect : SingleAbilityEffect
{
    public override string Id => "effect_single_damage";
    [Export] private float _hp = 10f;

    public override void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight) =>
        Damage(entity, gatherWeight * actuateWeight);

    public override void LauncherEffect(IEntity entity, float actuateWeight) =>
        Damage(entity, actuateWeight);

    private void Damage(IEntity entity, float size) =>
        entity.Health.Damage(Mathf.FloorToInt(_hp * size));
}