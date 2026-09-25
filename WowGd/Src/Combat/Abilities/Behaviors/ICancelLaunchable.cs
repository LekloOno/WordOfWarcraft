using System;
using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Behaviors.Data;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities.Behaviors;

public interface ICancelLaunchable : IAbility
{
    public ICancelLaunchableData CancelLaunchableData { get; }

    event Action<IEnumerable<ILaunch>>? CancelLaunchesEmitted;
}