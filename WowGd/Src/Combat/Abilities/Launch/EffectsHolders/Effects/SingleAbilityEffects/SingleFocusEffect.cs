using Godot;
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
            target.ResourceManager.Focus?.Generate(fp, out _);
        else
            target.ResourceManager.Focus?.Consume(-fp, out _);
    }
}