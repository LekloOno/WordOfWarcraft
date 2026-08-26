using System.Collections.Generic;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.AreaGatherers;

[GlobalClass]
public abstract partial class AreaGatherer : Node, IAreaGatherer
{
    public abstract HashSet<IEntity> Retrieve(Vector2 position);
}