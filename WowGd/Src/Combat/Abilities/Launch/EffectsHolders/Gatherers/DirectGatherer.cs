using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;

public class DirectGatherer : IGatherer
{
    public TargetsPayload Gather(ActuatePayload actuatePayload, TargetIntent intent)
    {
        if (intent.TryInto(actuatePayload.Caster, out Target target))
            return new ([target], actuatePayload.Caster, actuatePayload.Weight);

        return new ([], actuatePayload.Caster, actuatePayload.Weight); 
    }
}