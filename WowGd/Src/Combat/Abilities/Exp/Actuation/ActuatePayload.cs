using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Actuation;

public readonly struct ActuatePayload(IEntity entity, TargetIntent intent, float weight)
{
    public readonly IEntity         Entity = entity;
    public readonly TargetIntent    Intent = intent;
    public readonly float           Weight = weight;
}