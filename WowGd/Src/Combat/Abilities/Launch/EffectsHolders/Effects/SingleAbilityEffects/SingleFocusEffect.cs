using CombatCore;
using CombatCore.Primitives;
using CombatCore.Sources;
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

    protected override void Effect(
        IEntity launcher,
        IEntity target,
        Source source,
        bool IsDirect,
        float gatherWeight,
        float actuateWeight,
        Source? cause)
    {
        int fp = Mathf.FloorToInt(_focus * gatherWeight * actuateWeight);
        int value = Mathf.FloorToInt(_focus * gatherWeight * actuateWeight);

        ResourceId resourceId = ResourceRegistry.GetId<IFocus>();
        
        ResourceDirection dir = fp > 0 ? ResourceDirection.Add : ResourceDirection.Remove;

        if (dir is ResourceDirection.Remove)
            value *= -1;

        CombatCoreNode.Authority.Submit(new ResourceDraft(
            source, target.Id,
            resourceId, dir, value,
            RequestTagSet,
            cause));
    }
}