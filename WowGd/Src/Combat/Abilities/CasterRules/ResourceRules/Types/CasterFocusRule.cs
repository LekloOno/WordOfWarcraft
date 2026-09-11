using Godot;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.CasterRules.ResourceRules.Types;

[GlobalClass]
public partial class CasterFocusRule : CasterResourceRule
{
    public override string Id => "caster_rule_focus";

    public override bool Check(IEntity caster)
    {
        if (caster.ResourceManager.Focus is not IFocus focus)
            return false;

        return this.Compare(focus.Current, focus.Max, focus.Base);
    }
}