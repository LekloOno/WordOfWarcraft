using Godot;
using WowGd.Src.Combat.Abilities.Exp.CasterPreconditions;
using WowGd.Src.Combat.Abilities.Exp.Data.Models;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Data.Resources.CasterPreconditions.ResourceChecks;

[GlobalClass]
public partial class CasterHealthPreconditionData : CasterResourcePreconditionData, ICasterPreconditionData, ICasterPrecondition, IComponentFactory<ICasterPrecondition>
{
    public override string Id => "caster_precondition_health";

    public override bool Check(IEntity caster) =>
        this.Compare(caster.Health.CurrentHps, caster.Health.MaxHps, caster.Health.MaxHps);
}