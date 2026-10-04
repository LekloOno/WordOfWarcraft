using System;
using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Launch;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities;

public interface IAbility : IDisablable
{
    IAbilityData Data { get; }
    Task<bool> Start(IEntity caster);
    bool Cancel(IEntity caster);
    /// <summary>
    /// Allows to attach  additionnal launches to different hooks of a spell cast, for example as modifier.
    /// 
    /// It could be a passive ability that adds a projectile on every main launch.
    /// </summary>
    /// <param name="launch"></param>
    /// <param name="hook"></param>
    void AttachExternalLaunch(ILaunch launch, AbilityLaunchHook hook);

    event Action? Started;
    event Action? Stopped;
    event Action? Cancelled;
    
    event Action? TargetingStarted;
    event Action? TargetingCompleted;
    public event Action<TargetFailure>? TargetingFailed;
}

public enum AbilityLaunchHook
{
    Instant,
    Targeting,
    Activation,
    Main,
}