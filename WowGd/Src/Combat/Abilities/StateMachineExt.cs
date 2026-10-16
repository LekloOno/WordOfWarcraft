using System.Collections.Generic;
using CombatCore;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities;

public static class StateMachineExt
{
    public static void LaunchAll(this ICollection<ILaunch> launches, ActuatePayload payload, SpellInstance spellInstance)
    {
        foreach (ILaunch launch in launches)
            launch.Launch(payload, spellInstance);
    }
}