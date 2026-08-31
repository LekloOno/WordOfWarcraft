using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Actuation;

public readonly struct ActuatePayload(IEntity caster, TargetIntent intent, float weight)
{
    public readonly IEntity         Caster = caster;
    public readonly TargetIntent    Intent = intent;
    public readonly float           Weight = weight;
}