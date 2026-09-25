using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities.Handlers;

public interface ICancelLaunchableHandler
{
    void OnCancelLaunchesEmitted(IEnumerable<ILaunch> launches);
}