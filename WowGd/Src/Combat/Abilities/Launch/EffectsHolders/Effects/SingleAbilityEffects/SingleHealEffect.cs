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

    public override void Effect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight) =>
        target.Health.Generate(Mathf.FloorToInt(_hp * gatherWeight * actuateWeight), out _);
}