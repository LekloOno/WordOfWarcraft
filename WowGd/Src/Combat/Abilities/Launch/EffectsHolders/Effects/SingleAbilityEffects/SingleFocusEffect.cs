using CombatCore;
using Godot;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public partial class SingleFocusEffect : SingleAbilityEffect
{
    public override string Id => "effect_single_focus";
    [Export] private float _focus = 10f;

    public override void Effect(IEntity launcher, IEntity target, SpellInstance spellInstance, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        int fp = Mathf.FloorToInt(_focus * gatherWeight * actuateWeight);
        if (fp > 0)
            spellInstance.Request(
                target.Id,
                ResourceRegistry.GetId<IFocus>(),
                CombatCore.Primitives.Role.Feed,
                Mathf.FloorToInt(_focus * gatherWeight * actuateWeight),
                RequestTagSet);
        else
            spellInstance.Request(
                target.Id,
                ResourceRegistry.GetId<IFocus>(),
                CombatCore.Primitives.Role.Drain,
                Mathf.FloorToInt(-_focus * gatherWeight * actuateWeight),
                RequestTagSet);
    }
}