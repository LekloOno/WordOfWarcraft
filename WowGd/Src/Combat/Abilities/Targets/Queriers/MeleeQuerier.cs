using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Combat.Abilities.Targets.Queriers.Exceptions;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers;

/// <summary>
/// A querier that fires from the entity(ies) the requester is in melee with.
/// </summary>
[GlobalClass]
public partial class MeleeQuerier : TargetQuerier
{
    /// <summary>
    /// Fires from the entity(ies) the requester is in melee with.
    /// 
    /// <para>
    /// <b>Response time:</b>
    /// It is expected to be instantaneous. 
    /// Eventually have very minimal delay if its external source implements some kind of buffering behavior. 
    /// </para>
    /// 
    /// <para>
    /// <b>Failure:</b>
    /// It might fail if the requester is not in melee.
    /// </para>
    /// 
    /// </summary>
    /// <param name="requester">The query requester.</param>
    /// <returns>The resulting targets payload.</returns>
    /// <exception cref="TargetQueryCancelledException"></exception>
    /// <exception cref="TargetQueryTimedOutException"></exception>
    public override Task<TargetsPayload> Query(IEntity requester)
    {
        throw new TargetQueryCancelledException();
    }
}