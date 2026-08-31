using System;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Targeting;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;

public interface ITrigger
{
    void Start(ActuatePayload payload);
    /// <summary>
    /// Emitted when the rest of the launch should be triggered.
    /// The second TargetIntent is the effect target, computed by the trigger.
    /// 
    /// For example, if it's a fire ball, it's the target it actually did hit.
    /// </summary>
    event Action<ActuatePayload, TargetIntent>? Triggered;
}