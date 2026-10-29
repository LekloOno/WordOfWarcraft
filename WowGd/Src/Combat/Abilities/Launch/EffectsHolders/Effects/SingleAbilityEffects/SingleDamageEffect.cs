using CombatCore;
using CombatCore.Sources;
using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources;
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

    protected override void Effect(
        IEntity launcher,
        IEntity target,
        Source source,
        bool IsDirect,
        float gatherWeight,
        float actuateWeight,
        Source? cause) =>
        CombatCoreNode.Authority.Submit(new ResourceDraft(
            source, target.Id,
            ResourceRegistry.GetId<IEntityHealth>(),
            ResourceDirection.Remove,
            Mathf.FloorToInt(_hp * gatherWeight * actuateWeight),
            RequestTagSet,
            cause));
}