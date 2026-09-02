using System;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Triggers;

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