using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Data;

namespace WowGd.Src.Combat.Abilities.Behaviors.Data;

public interface IStopLaunchableData
{
    /// <summary>
    /// The launches to emit once the looping rules can't be met anymore.
    /// <br/>
    /// Order has a meaning. First launch will be launch first.
    /// A launch might thus invalidate the conditions of the next one.
    /// </summary>
    IReadOnlyList<ILaunchData>  StopLaunches            { get; }
}