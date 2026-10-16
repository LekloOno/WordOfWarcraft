using System;
using CombatCore;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Triggers;

public class InstantTrigger : ITrigger
{
    public event Action<ActuatePayload, TargetIntent, SpellInstance>? Triggered;

    public void Start(ActuatePayload payload, SpellInstance spellInstance)
    {
        Triggered?.Invoke(payload, payload.Intent, spellInstance);
    }
}