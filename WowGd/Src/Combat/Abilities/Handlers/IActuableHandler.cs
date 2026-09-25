using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities.Handlers;

public interface IActuableHandler
{
    void OnActuationLaunchesEmitted(IEnumerable<ILaunch> launches);
    void OnActuationStarted();
    void OnActuationCompleted();
}