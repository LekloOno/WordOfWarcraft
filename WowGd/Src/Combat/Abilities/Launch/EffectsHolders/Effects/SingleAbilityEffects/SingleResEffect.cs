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

    public override void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight) =>
        Resurrect(entity, gatherWeight * actuateWeight);

    public override void LauncherEffect(IEntity entity, float actuateWeight) =>
        Resurrect(entity, actuateWeight);

    private void Resurrect(IEntity entity, float size)
    {
        int? hp = _hp > 0 ? Mathf.FloorToInt(_hp * size) : null;
        entity.Health.Resurrect(hp);
    }
}