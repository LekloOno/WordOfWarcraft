using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public partial class SingleResEffect : SingleAbilityEffect
{
    public override string Id => "effect_single_resurrect";
    [Export] private float _hp = 10f;

    public override void Effect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        int? hp = _hp > 0 ? Mathf.FloorToInt(_hp * gatherWeight * actuateWeight) : null;
        target.Health.Resurrect(hp);
    }
}