using Godot;
using System.Collections.Generic;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.AreaGatherers;

/// <summary>
/// Decouplates the means to retrieve indirect targets through space.
/// </summary>
public interface IAreaGatherer
{
    HashSet<IEntity> Retrieve(Vector2 position);
}