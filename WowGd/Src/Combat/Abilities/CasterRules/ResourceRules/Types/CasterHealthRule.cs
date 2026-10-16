using Godot;
using WowGd.Src.Combat.Health;

namespace WowGd.Src.Combat.Abilities.CasterRules.ResourceRules.Types;

[GlobalClass]
public partial class CasterHealthRule : CasterResourceRule<IEntityHealth>
{
    public override string Id => "caster_rule_health";
}