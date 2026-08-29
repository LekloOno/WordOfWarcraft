using Godot;
using WowGd.Src.Combat.Abilities.Exp.CasterPreconditions;
using WowGd.Src.Combat.Abilities.Exp.Data.Models;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Data.Resources.CasterPreconditions;

[GlobalClass]
public abstract partial class CasterPrecondition : Resource, ICasterPreconditionData, ICasterPrecondition, IComponentFactory<ICasterPrecondition>
{
    // Precondition should not mutate anything, and be stateless, so the data is itself the logic.
    public ICasterPrecondition Build() => this;

    public abstract string Id { get; }
    public abstract bool Check(IEntity caster);
}