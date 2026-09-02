using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Data;

public interface ITargetRule
{
    string Id { get; }
    bool Check(IEntity caster, TargetIntent intent);
}