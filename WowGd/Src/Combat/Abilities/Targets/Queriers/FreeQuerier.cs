using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Combat.Abilities.Targets.Queriers.Exceptions;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers;

/// <summary>
/// A querier that fires from a free position that must be retrieved externally.
/// </summary>
[GlobalClass]
public partial class FreeQuerier : TargetQuerier
{
    /// <summary>
    /// Fires from a free external position.
    /// 
    /// <para>
    /// <b>Response time:</b>
    /// For a human player, that means waiting for some UI interraction to retrieve a selected position.
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