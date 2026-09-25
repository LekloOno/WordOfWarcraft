using WowGd.Src.Combat.Abilities.Data;

namespace WowGd.Src.Combat.Abilities.Behaviors.Data;

/// <summary>
/// A stateless representation of an ability, pure data.
/// </summary>
public interface IAbilitySmData :
    IAbilityData,
    IInstantLaunchableData,
    ITargetLaunchableData,
    IActuableData,
    IStopLaunchableData,
    ICancelLaunchableData,
    ICoolDownData;