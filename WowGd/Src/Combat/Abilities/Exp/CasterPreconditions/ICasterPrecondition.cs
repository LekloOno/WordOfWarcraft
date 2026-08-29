using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.CasterPreconditions;

public interface ICasterPrecondition
{
    bool Check(IEntity caster);
}