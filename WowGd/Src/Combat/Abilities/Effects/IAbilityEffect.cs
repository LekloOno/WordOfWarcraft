using WowGd.Src.Combat.Abilities.Targets.Payload;

namespace WowGd.Src.Combat.Abilities.Effects;

/// <summary>
/// Describes the effects applied to its targets.
/// 
/// Damages, heal, status, and any other more complex effect that can be applied to an entity.
/// </summary>
public interface IAbilityEffect
{
    bool Apply(TargetsPayload targetsPayload, float size);
}