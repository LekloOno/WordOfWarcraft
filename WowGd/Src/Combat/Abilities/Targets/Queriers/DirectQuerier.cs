using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Combat.Abilities.Targets.Queriers.Exceptions;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers;

/// <summary>
/// A querier that fires from explicit targetted entity(ies).
/// </summary>
[GlobalClass]
public partial class DirectQuerier : TargetQuerier
{
    /// <summary>
    /// Fires from explicit targetted entity(ies).
    /// 
    /// <para>
    /// <b>Response time:</b>
    /// For a human player, that means the response time might be
    /// <list type="bullet">
    ///     <item><i>Instateneous</i> if there's an already accessible buffered locked target.</item>
    ///     <item><i>Delayed</i> otherwise, through some UI interraction to retrieve the entity(ies).</item>
    /// </list>
    /// </para>
    /// 
    /// <para>
    /// <b>Failure:</b>
    /// For a human player, that means the query might fail if selection is cancelled.
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