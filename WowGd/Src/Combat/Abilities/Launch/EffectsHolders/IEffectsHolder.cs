using WowGd.Src.Combat.Abilities.Actuation;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders;

public interface IEffectsHolder
{
    void Launch(ActuatePayload payload);
}