using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Combat.Abilities.Targets.Queriers.Exceptions;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers;

/// <summary>
/// A querier that fires from the requester itself, thus doesn't rely on any external data.
/// </summary>
[GlobalClass]
public partial class SelfQuerier : TargetQuerier
{
    /// <summary>
    /// Fires from the requester itself, thus doesn't rely on any external data.
    /// 
    /// <para>
    /// <b>Response time:</b>
    /// It is always instantaneous.
    /// </para>
    /// 
    /// <para>
    /// <b>Failure:</b>
    /// It never fails.
    /// </para>
    /// 
    /// </summary>
    /// <param name="requester">The query requester.</param>
    /// <returns>The resulting targets payload.</returns>
    /// <exception cref="TargetQueryCancelledException"></exception>
    /// <exception cref="TargetQueryTimedOutException"></exception>
    public override Task<TargetsPayload> Query(IEntity requester)
    {
        return Task.FromResult(
            new TargetsPayload(RetrieveTargets(requester), requester)
        );
    }

    private HashSet<Target> RetrieveTargets(IEntity requester)
    {
        if (_areaGatherer is null)
            return [];
        
        HashSet<Target> targets = [];

        foreach (IEntity entity in _areaGatherer.Retrieve(requester.Body.GlobalPosition))
        {
            Target target;

			if (requester == entity)
				target = new(entity, TargetRelation.Self, false);
			else if (requester.TeamMask == entity.TeamMask)
				target = new(entity, TargetRelation.Ally, false);
			else
				target = new(entity, TargetRelation.Enemy, false);
			
			targets.Add(target);
        }

        return targets;
    }
}