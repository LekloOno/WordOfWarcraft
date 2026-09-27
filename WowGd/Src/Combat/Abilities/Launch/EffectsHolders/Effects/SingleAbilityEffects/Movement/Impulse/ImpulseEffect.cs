using Godot;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.Impulse;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects;

/// <summary>
/// Applies an impulse in the external channel of the target entities.
/// 
/// For gathered targets, the direction is the direction from the launcher to the target.
/// For the launcher, the direction is its wish dir.
/// </summary>
[GlobalClass]
public partial class ImpulseEffect : SingleAbilityEffect
{
    public override string Id => "effect_impulse";
    
    [Export(PropertyHint.Range, "0,100,0.1,or_greater,exp")]
    private float _strength = 15f;
    [Export] private ImpulseMode _impulseMode = ImpulseMode.Push;

    public override void Effect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        Vector2 direction = (launcher.Body.GlobalPosition - target.Body.GlobalPosition).Normalized();
        if (_impulseMode is ImpulseMode.Push)
            direction *= -1f;

        DoImpulse(target, direction, gatherWeight, actuateWeight);
    }

    public override void LauncherEffect(IEntity entity, float actuateWeight)
    {
        Vector2 direction = entity.WishDir.WishDir();
        if (_impulseMode is ImpulseMode.Pull)
            direction *= -1f;

        DoImpulse(entity, direction, 1f, actuateWeight);
    }

    private void DoImpulse(IEntity target, Vector2 direction, float gatherWeight, float actuateWeight)
    {
        float strength = _strength * gatherWeight * actuateWeight;
        new ImpulseContrib(target.EntityMover, direction, strength).Start();
    }
}