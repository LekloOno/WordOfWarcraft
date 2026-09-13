using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.CasterRules.ResourceRules.Types;

[GlobalClass]
public partial class CasterHealthRule : CasterResourceRule
{
    public override string Id => "caster_rule_health";

    public override bool Check(IEntity caster) =>
        this.Compare(caster.Health.Current, caster.Health.Max, caster.Health.Base);
}