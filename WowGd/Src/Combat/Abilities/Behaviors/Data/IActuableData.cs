using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Data;

namespace WowGd.Src.Combat.Abilities.Behaviors.Data;

public interface IActuableData
{
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
    /// Condition to meet to loop back to the actuator, about the actuation.
    /// <br/>
    /// Order does not matter, preconditions should not mutate anything.
    /// </summary>
    IReadOnlyList<ILoopRule>    LoopRules               { get; }
    /// <summary>
    /// Condition to meet to loop back to the actuator, about the caster.
    /// <br/>
    /// Order does not matter, preconditions should not mutate anything.
    /// </summary>
    IReadOnlyList<ICasterRule>  CasterLoopRules         { get; }
    /// <summary>
    /// Condition to meet to loop back to the actuator, about the targeting.
    /// <br/>
    /// Order does not matter, preconditions should not mutate anything.
    /// </summary>
    IReadOnlyList<ITargetRule>  TargetLoopRules         { get; }
}