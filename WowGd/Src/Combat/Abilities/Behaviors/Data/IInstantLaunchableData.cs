using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Data;

namespace WowGd.Src.Combat.Abilities.Behaviors.Data;

public interface IInstantLaunchableData
{
    /// <summary>
    /// The launches to emit if the start preconditions are met.
    /// <br/>
    /// Since there's no target intent yet, it will be emitted with the caster informations only.
    /// <br/>
    /// Order has a meaning. First launch will be launch first.
    /// A launch might thus invalidate the conditions of the next one.
    /// </summary>
    IReadOnlyList<ILaunchData>  InstantLaunches         { get; }
}