using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Combat.Abilities.Exp.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Gatherers;

public interface IGatherer
{
    TargetsPayload Gather(ActuatePayload actuatePayload, TargetIntent intent);
}