using WowGd.Src.Combat.Abilities.Exp.Launch;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Exp;

public interface IAbility : IDisablable
{
    void Start(IEntity caster);
    bool Cancel(IEntity caster);
    /// <summary>
    /// Allows to attach  additionnal launches to different hooks of a spell cast, for example as modifier.
    /// 
    /// It could be a passive ability that adds a projectile on every main launch.
    /// </summary>
    /// <param name="launch"></param>
    /// <param name="hook"></param>
    void AttachExternalLaunch(ILaunch launch, AbilityLaunchHook hook);
}

public enum AbilityLaunchHook
{
    Instant,
    Targeting,
    Activation,
    Main,
}