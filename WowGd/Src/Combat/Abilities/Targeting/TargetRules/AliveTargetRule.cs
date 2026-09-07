using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting.TargetRules;

[GlobalClass]
public partial class AliveTargetRule : TargetRule
{
    [Export] private bool _alive = true;
    public override string Id => "target_rule_alive";

    public override bool Check(IEntity caster, TargetIntent intent)
    {
        if (!intent.IsDirect || intent.Entity is null)
            return true;

        return intent.Entity.Health.Dead() != _alive;
    }
}