using Godot;
using WowGd.Src.Combat.Resources.FocusRes;

namespace WowGd.Src.Combat.Abilities.CasterRules.ResourceRules.Types;

[GlobalClass]
public partial class CasterFocusRule : CasterResourceRule<IFocus>
{
    public override string Id => "caster_rule_focus";
}