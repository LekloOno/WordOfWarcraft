using System.Collections.Generic;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.AreaGatherers;

[GlobalClass]
public partial class RadiusGatherer : AreaGatherer
{
    [Export] private float _radius = 5f;

    public override HashSet<IEntity> Retrieve(Vector2 position) =>
        EntitiesRegistry.GetEntitiesInRange(position, _radius);
}