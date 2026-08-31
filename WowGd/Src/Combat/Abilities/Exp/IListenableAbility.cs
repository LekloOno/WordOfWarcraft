using System;
using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Exp.Launch;

namespace WowGd.Src.Combat.Abilities.Exp;

public interface IListenableAbility : IAbility
{
    event Action? Started;
    event Action? Stopped;
    event Action? Cancelled;
    event Action? PreconditionsPassed;
    //event Action.. PreconditionsFailed;
    event Action<List<ILaunch>> InstantLaunchesEmitted;
    event Action? TargetingStarted;
    event Action? TargetingCompleted;
    event Action<List<ILaunch>> TargetingLaunchesEmitted;
    event Action<List<ILaunch>> ActivationLaunchesEmitted;
    event Action? ActivationStarted;
    event Action? ActivationCompleted;
    event Action<List<ILaunch>> MainLaunchesEmitted;
}