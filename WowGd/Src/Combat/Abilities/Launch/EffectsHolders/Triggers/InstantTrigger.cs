using System;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Triggers;

public class InstantTrigger : ITrigger
{
    public event Action<ActuatePayload, TargetIntent>? Triggered;

    public void Start(ActuatePayload payload)
    {
        Triggered?.Invoke(payload, payload.Intent);
    }
}