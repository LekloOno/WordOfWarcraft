using Godot;
using WowGd.Src.Combat.Abilities.Actuation;

namespace WowGd.Src.Combat.Abilities.LoopRules;

[GlobalClass]
public partial class AlwaysLoopRule : LoopRule
{
    public override string Id => "loop_rule_always";
    public override bool Check(ActuatePayload payload) => true;
}