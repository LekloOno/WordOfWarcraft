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

    public override void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight) =>
        AffectFocus(entity, gatherWeight * actuateWeight);

    public override void LauncherEffect(IEntity entity, float actuateWeight) =>
        AffectFocus(entity, actuateWeight);

    private void AffectFocus(IEntity entity, float size)
    {
        int fp = Mathf.FloorToInt(_focus * size);
        if (fp > 0)
            entity.ResourceManager.Focus?.Generate(fp, out _);
        else
            entity.ResourceManager.Focus?.Consume(-fp, out _);
    }
}