using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp;

public readonly struct CastPayload(IEntity entity, TargetIntent intent, float weight)
{
    public readonly IEntity         Entity = entity;
    public readonly TargetIntent    Intent = intent;
    public readonly float           Weight = weight;
}