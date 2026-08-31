using WowGd.Src.Combat.Abilities.Exp.Actuation;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

public interface IEffectsHolder
{
    void Launch(ActuatePayload payload);
}