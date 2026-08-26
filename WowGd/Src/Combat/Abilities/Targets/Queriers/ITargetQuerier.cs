using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers;

/// <summary>
/// Decouplates the means to retrieve abilities targets.
/// </summary>
public interface ITargetQuerier
{
    Task<TargetsPayload> Query(IEntity requester);
}