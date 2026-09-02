using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting.TargetRules;

[GlobalClass]
public partial class RangeTargetRule : TargetRule
{
    public override string Id => "target_rule_distance";
    [Export] private float _minDistance = 0f;
    [Export] private float _maxDistance = 10f;

    public override bool Check(IEntity caster, TargetIntent intent)
    {
        float sqDistance = caster.Body.GlobalPosition.DistanceSquaredTo(intent.Position);

        return sqDistance < _maxDistance * _maxDistance && sqDistance > _minDistance * _maxDistance;
    }
}