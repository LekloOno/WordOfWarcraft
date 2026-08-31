using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Launch;

namespace WowGd.Src.Combat.Abilities.Exp;

public static class StateMachineExt
{
    public static void LaunchAll(this ICollection<ILaunch> launches, ActuatePayload payload)
    {
        foreach (ILaunch launch in launches)
            launch.Launch(payload);
    }
}