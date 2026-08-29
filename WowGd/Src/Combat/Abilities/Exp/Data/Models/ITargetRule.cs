using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Data.Models;

public interface ITargetRule
{
    string Id { get; }
    bool Check(IEntity caster, TargetIntent intent);
}