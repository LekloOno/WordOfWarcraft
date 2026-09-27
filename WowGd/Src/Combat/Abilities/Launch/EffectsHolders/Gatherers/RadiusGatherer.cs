using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;

public class RadiusGatherer(RadiusGathererData data) : IGatherer
{
    private readonly RadiusGathererData _data = data;

    public TargetsPayload Gather(ActuatePayload actuatePayload, TargetIntent intent)
    {
        ICollection<(IEntity, float)> entities;

        Target[] targets;
        IEntity caster = actuatePayload.Caster;

        if (intent.TryGetEntity(out IEntity? direct))
        {
            entities = GetEntitiesInRange(caster, direct.Body.GlobalPosition, _data.Radius, direct);

            if (_data.Settings.HasFlag(RadiusGathererSettings.IncludeDirect))
            {
                targets = new Target[entities.Count + 1];
                targets[^1] = new(direct, direct.GetRelationTo(caster), true, 1f);
            }
            else
                targets = new Target[entities.Count];
        }
        else
        {
            entities = GetEntitiesInRange(caster, intent.Position, _data.Radius);
            targets = new Target[entities.Count];
        }

        int idx = 0;
        foreach ((IEntity entity, float distance) in entities)
        {
            float weight = _data.Settings.HasFlag(RadiusGathererSettings.WeightOnDistance) ?
                (_data.Radius - distance)/_data.Radius : 1f;

            targets[idx++] = new(entity, entity.GetRelationTo(caster), false, weight);
        }

        return new (targets, caster, actuatePayload.Weight); 
    }

    private bool Check(Vector2 from, IEntity caster, IEntity target)
    {
        uint collisionMask = caster.ToCollisionMask(_data.Settings);

        if (collisionMask == 0)
            return true;

        Vector2 to = target.Body.GlobalPosition;

        if (from == to)
            return true;

        var exclude = new Godot.Collections.Array<Rid>
        {
            target.Body.GetRid()
        };

        if (!_data.Settings.HasFlag(RadiusGathererSettings.SelfObstruction))
            exclude.Add(caster.Body.GetRid());


        var query = PhysicsRayQueryParameters2D.Create(
            from,
            to,
            exclude: exclude,
            collisionMask: collisionMask
        );

        var spaceState = caster.Body.GetWorld2D().DirectSpaceState;
        var hit = spaceState.IntersectRay(query);

        return hit.Count == 0;
    }


    public ICollection<(IEntity, float)> GetEntitiesInRange(IEntity caster, Vector2 position, float range, IEntity? exclude = null)
    {
        ICollection<(IEntity, float)> inRange = [];
        float rangeSquared = range * range;
        
        foreach (IEntity entity in EntitiesRegistry.Entities)
        {
            if (entity == exclude)
                continue;

            float squaredDist = (entity.Body.GlobalPosition - position).LengthSquared();
            if (squaredDist <= rangeSquared && Check(position, caster, entity))
                inRange.Add((entity, Mathf.Sqrt(squaredDist)));
        }

        return inRange;
    }
}