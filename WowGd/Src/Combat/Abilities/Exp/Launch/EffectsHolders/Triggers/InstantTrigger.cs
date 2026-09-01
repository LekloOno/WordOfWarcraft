using System;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Targeting;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;

public class InstantTrigger : ITrigger
{
    public event Action<ActuatePayload, TargetIntent>? Triggered;

    public void Start(ActuatePayload payload)
    {
        Triggered?.Invoke(payload, payload.Intent);
    }
}