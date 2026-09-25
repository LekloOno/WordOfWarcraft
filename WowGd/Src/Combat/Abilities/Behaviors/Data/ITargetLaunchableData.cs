using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Data;

namespace WowGd.Src.Combat.Abilities.Behaviors.Data;

public interface ITargetLaunchableData
{
    /// <summary>
    /// The launches to emit when the target has been acquired.
    /// <br/>
    /// Order has a meaning. First launch will be launch first.
    /// A launch might thus invalidate the conditions of the next one.
    /// </summary>
    IReadOnlyList<ILaunchData>  TargetingLaunches       { get; }
}