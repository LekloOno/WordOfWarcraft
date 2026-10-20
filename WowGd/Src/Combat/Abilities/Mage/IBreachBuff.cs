using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Mage;

public interface IBreachBuff
{
    string Key { get; }
    void Apply(IEntity entity, TargetRelation targetRelation);
    void Cancel(IEntity entity, TargetRelation targetRelation);
}