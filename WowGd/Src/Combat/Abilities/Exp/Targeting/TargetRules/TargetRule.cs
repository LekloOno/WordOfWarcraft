using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.CasterRules;

[GlobalClass]
public abstract partial class TargetRule : Resource, ITargetRule
{
    public abstract string Id { get; }
    public abstract bool Check(IEntity caster, TargetIntent intent);
}