using System.Collections.Generic;
using Godot;
using WowGd.Src.Dactylo.Abilities.Targets;
using WowGd.Src.Entities;

namespace WowGd.Src.Dactylo.Abilities.Effects;

[GlobalClass]
public partial class TestRadiusAttack : FreeTargetAbilityEffect
{
    [Export] private float _radius = 5f;
    [Export] private float _damage = 10f;

    public override bool Apply(IEntity launcher, FreeTarget target, float size)
    {
        float effectiveDmg = _damage * size;
        ICollection<IEntity> targets = launcher.GetTargetsInRange(_radius, target.TeamMask);
        foreach (IEntity entity in targets)
            entity.Health.Damage(Mathf.FloorToInt(effectiveDmg));

        return true;
    }
}