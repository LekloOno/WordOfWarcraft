using WowGd.Src.Dactylo.Abilities.Targets;
using WowGd.Src.Entities;

namespace WowGd.Src.Dactylo.Abilities.Effects;

public interface IAbilityEffect<T>
where
    T: ITarget
{
    bool Apply(IEntity launcher, T target, float size);
}