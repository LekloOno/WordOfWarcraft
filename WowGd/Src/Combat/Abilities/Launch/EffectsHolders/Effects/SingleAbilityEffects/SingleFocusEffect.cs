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

    public override void Effect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        int fp = Mathf.FloorToInt(_focus * gatherWeight * actuateWeight);
        if (fp > 0)
            CombatCoreNode.Authority.Submit(new RequestDraft(
                new(CombatCore.Primitives.SourceKind.Ability, 1, launcher.Id),
                target.Id,
                ResourceRegistry.GetId<IFocus>(),
                CombatCore.Primitives.Role.Heal,
                Mathf.FloorToInt(_focus * gatherWeight * actuateWeight)));
        else
            CombatCoreNode.Authority.Submit(new RequestDraft(
                new(CombatCore.Primitives.SourceKind.Ability, 1, launcher.Id),
                target.Id,
                ResourceRegistry.GetId<IFocus>(),
                CombatCore.Primitives.Role.Drain,
                Mathf.FloorToInt(-_focus * gatherWeight * actuateWeight)));
    }
}