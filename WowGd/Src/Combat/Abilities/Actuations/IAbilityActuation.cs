using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Targets.Payload;

namespace WowGd.Src.Combat.Abilities.Actuations;

/// <summary>
/// Describes the means to activate an ability.
/// 
/// It could be a simple instant cast, a time/dactylography cast, etc.
/// </summary>
public interface IAbilityActuation
{
    Task<bool> Actuate(TargetsPayload targetsPayload, float size, out float resultSize);
}