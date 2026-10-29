using System;
using CombatCore.Sources;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Triggers;

public class InstantTrigger : ITrigger
{
    public event Action<ActuatePayload, TargetIntent, Source, Source?>? Triggered;

    public void Start(ActuatePayload payload, Source source, Source? cause)
    {
        Triggered?.Invoke(payload, payload.Intent, source, cause);
    }
}