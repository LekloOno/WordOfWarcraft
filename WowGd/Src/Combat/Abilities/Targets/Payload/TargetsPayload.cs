using System.Collections.Generic;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.Payload;

/// <summary>
/// Represents the payload resulting of a targeting query.
/// The launcher is the entity at the initiative of the target query, typically of the spell launch.
/// 
/// This entity might appear twice - if the targeting query used an area.
/// It might then be included in the `_targets`. 
/// </summary>
/// <param name="targets">The resulting targets of the query.</param>
/// <param name="launcher">The entity at the initiative of the target query.</param>
public readonly struct TargetsPayload(HashSet<Target> targets, IEntity launcher)
{
    private readonly HashSet<Target> _targets = targets;
    public readonly IEntity Launcher = launcher;
    public readonly IReadOnlySet<Target> Targets => _targets; 
}