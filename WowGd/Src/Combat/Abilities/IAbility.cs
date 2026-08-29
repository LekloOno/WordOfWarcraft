using System;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities;

/// <summary>
/// Abilities work in a kind of state machine execution flow.
/// 
/// Ability
/// | _____                    
/// | Precondition(s) (sync)                - NO -------------------------> END
/// |   - cooldowns, cost anticipation, status, thresholds ..
/// | 
/// | YES - if all preconditions are valid
/// | _____
/// | Target Intent Acquisition (async)     - NO -------------------------> END
/// |   - IA decision, user inputs..
/// |       - results in free position or direct entities targeting
/// |
/// | YES - if target can be acquired
/// | _____
/// | Actuation(s) (async)                  - NO -------------------------> END
/// |   - Instant (sync), cast time, dactylo ..                     <-----------------------------------+
/// |                                                                                                   |
/// | YES - (independantly for each actuator) if actuation succeeded | (once for all) if any actuation succeeded -------->  Restart Condition
/// | _____
/// | IAbilityTrigger(s) (async)
/// |   - a projectile, a hitscan, an unconditionnal hit..
/// |       - it computes the effective intent targets. Hit entities, hit position..
/// | 
/// | YES - (independantly for each trigger) 
/// </summary>
public interface IAbility : IDisablable
{
    bool Start();
    bool Stop();

    event Action? Started;
    event Action? Stopped;
}