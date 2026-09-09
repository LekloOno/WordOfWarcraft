using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities;

public interface IAbilityLaunchesHandler
{
    //event Action.. PreconditionsFailed;
    void OnInstantLaunchesEmitted(IEnumerable<ILaunch> launches);
    void OnTargetingLaunchesEmitted(IEnumerable<ILaunch> launches);
    void OnActuationLaunchesEmitted(IEnumerable<ILaunch> launches);
}
