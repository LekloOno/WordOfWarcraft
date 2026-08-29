using System.Threading.Tasks;

namespace WowGd.Src.Combat.Abilities.Targets.Intents;

/// <summary>
/// The role of this stage is to acquire the intended targets of the abilities.
/// 
/// It is an "intent" in that it won't necessarily be the effective targets of the ability.
/// For example, if the  ability uses a projectile, it might not reach such targets, be destroyed before, hit a wall, etc.
/// 
/// There's a single intent acquirer in an ability execution graph.
/// </summary>
public interface ITargetIntentAcquirer
{
    Task<TargetIntent> AcquireIntent(CastContext context);
}