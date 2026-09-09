using System;
using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities;

public interface IListenableAbility : IAbility
{
    event Action? Started;
    event Action? Stopped;
    event Action? Cancelled;
    event Action<IEnumerable<ILaunch>>? CancelLaunchesEmitted;
    event Action<ulong>? CoolDownStarted;
    event Action? CoolDownCancelled;
    event Action<IEnumerable<ILaunch>>? InstantLaunchesEmitted;
    event Action? TargetingStarted;
    event Action? TargetingCompleted;
    event Action<IEnumerable<ILaunch>>? TargetingLaunchesEmitted;
    event Action<IEnumerable<ILaunch>>? ActuationLaunchesEmitted;
    event Action? ActuationStarted;
    event Action? ActuationCompleted;
}