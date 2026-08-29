using System.Collections.Generic;

namespace WowGd.Src.Combat.Abilities.Exp.Data;

/// <summary>
/// A stateless representation of an ability, pure data.
/// </summary>
public interface IAbilityData
{
    string Id { get; }
    /// <summary>
    /// The list of conditions that must be passed to move to the next step.
    /// <br/>
    /// Order does not matter, preconditions should not mutate anything.
    /// </summary>
    IReadOnlyList<ICasterRule>  StartPreconditions      { get; }
    /// <summary>
    /// The launches to emit if the start preconditions are met.
    /// <br/>
    /// Since there's no target intent yet, it will be emitted with the caster informations only.
    /// <br/>
    /// Order has a meaning. First launch will be launch first.
    /// A launch might thus invalidate the conditions of the next one.
    /// </summary>
    IReadOnlyList<ILaunchData>  InstantLaunches         { get; }
    /// <summary>
    /// The rules that apply to acquire targets, relative to the states of the caster and given target intent.
    /// <br/>
    /// An intent is either a position, or another entity.
    /// <br/>
    /// Order does not matter, preconditions should not mutate anything.
    /// </summary>
    IReadOnlyList<ITargetRule>  TargetRules             { get; }
    /// <summary>
    /// The mean of acquiring targets.
    /// 
    /// Melee, direct, free or self targeting.
    /// It does not conduct the logic of targeting itself, but what component of the entity we're calling to retrieve such target.
    /// 
    /// This makes the ability description compatible with both an AI or a player.
    /// A player would redirect to an input based interraction, while the AI will retrieve it through its decision algorithm.
    /// </summary>
    TargetIntentAcquirer        TargetIntentAcquirer    { get; }
    /// <summary>
    /// The launches to emit when the target has been acquired.
    /// <br/>
    /// Order has a meaning. First launch will be launch first.
    /// A launch might thus invalidate the conditions of the next one.
    /// </summary>
    IReadOnlyList<ILaunchData>  TargetingLaunches       { get; }
    /// <summary>
    /// The way to actuate the main launches.
    /// 
    /// For example, a time cast, some interractions from the entity (input for player like dactylo, decisions for an AI), etc.
    /// </summary>
    IActuatorData               Actuator                { get; }
    /// <summary>
    /// The launches to emit once actuation has been completed.
    /// <br/>
    /// Order has a meaning. First launch will be launch first.
    /// A launch might thus invalidate the conditions of the next one.
    /// </summary>
    IReadOnlyList<ILaunchData>  ActuationLaunches       { get; }
    /// <summary>
    /// Condition to meet to loop back to the actuator.
    /// <br/>
    /// Order does not matter, preconditions should not mutate anything.
    /// </summary>
    IReadOnlyList<ILoopRule>    LoopRules               { get; }
}