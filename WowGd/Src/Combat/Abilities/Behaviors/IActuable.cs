using System;
using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Behaviors.Data;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities.Behaviors;

public interface IActuableAbility : IAbility
{
    public IActuableData ActuableData { get; }

    event Action<IEnumerable<ILaunch>>? ActuationLaunchesEmitted;
    event Action? ActuationStarted;
    event Action? ActuationCompleted;
}