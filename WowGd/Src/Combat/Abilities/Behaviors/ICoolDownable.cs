using WowGd.Src.Combat.Abilities.CoolDowns;

namespace WowGd.Src.Combat.Abilities.Behaviors;

public interface ICoolDownable : IAbility
{
    public ICoolDown CoolDown { get; }
}