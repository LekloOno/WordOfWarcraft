using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities.Handlers;

public interface IStopLaunchableHandler
{
    void OnStopLaunchesEmitted(IEnumerable<ILaunch> launches);
}