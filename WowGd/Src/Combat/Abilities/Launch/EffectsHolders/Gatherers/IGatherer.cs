using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;

public interface IGatherer
{
    TargetsPayload Gather(ActuatePayload actuatePayload, TargetIntent intent);
}