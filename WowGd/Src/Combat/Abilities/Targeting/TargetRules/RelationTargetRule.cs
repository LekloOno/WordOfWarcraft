using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting.TargetRules;

[GlobalClass]
public partial class RelationTargetRule : TargetRule
{
    [Export] private TargetRelation _relation;
    public override string Id => "target_rule_relation";

    
    public RelationTargetRule() {}
    public RelationTargetRule(TargetRelation relation) : this() { _relation = relation; }

    public override bool Check(IEntity caster, TargetIntent intent)
    {
        if (!intent.TryGetEntity(out IEntity? entity))
            return true;

        return (entity.GetRelationTo(caster) & _relation) != 0;
    }
}