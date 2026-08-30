using System;
using System.Threading;
using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Actuators;

/// <summary>
/// An actuator is a mean to actuate launches (called main/activation launches),
/// generally involving interractions from the entity, like dactylographgy (for a human player).
/// 
/// The exact interraction are abstracted away, as a responsibility of the entity.
/// This way, both AI and players can use the exact same ability architecture.
/// 
/// See more details in this communication procedure in sibling namespace "Actuation".
/// </summary>
public interface IActuator
{
    Task Actuate(IEntity entity, TargetIntent intent, CancellationToken ct);

    // using event instead of a Task because actuate might trigger at high frequency
    // For example in the case of a beam. Using task would stress out the GC, but we can't either
    // expect ValueTask to be effective in all scenarios. A push approach is thus probably better.
    event Action<ActuatePayload>? Actuated;
}