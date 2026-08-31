using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting.TargetRules;

[GlobalClass]
public partial class LosTargetRule : TargetRule
{
    public override string Id => "target_rule_los";

    public override bool Check(IEntity caster, TargetIntent intent)
    {
        Vector2 from = caster.Body.GlobalPosition;
        Vector2 to = intent.Position;

        var exclude = new Godot.Collections.Array<Rid>
        {
            caster.Body.GetRid()
        };

        if (intent.IsDirect)
            exclude.Add(intent.Entity!.Body.GetRid());

        var query = PhysicsRayQueryParameters2D.Create(
            from,
            to,
            exclude: exclude
        );

        var spaceState = caster.Body.GetWorld2D().DirectSpaceState;
        var hit = spaceState.IntersectRay(query);

        return hit.Count == 0;
    }
}