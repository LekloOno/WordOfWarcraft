using WowGd.Src.Combat.Abilities.Targets.Payload;

namespace WowGd.Src.Combat.Abilities.Effects;

public interface IAbilityEffect
{
    bool Apply(TargetsPayload targetsPayload, float size);
}