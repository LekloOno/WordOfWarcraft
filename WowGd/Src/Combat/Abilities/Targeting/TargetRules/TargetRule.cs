using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting.TargetRules;

[GlobalClass]
public abstract partial class TargetRule : Resource, ITargetRule
{
    public abstract string Id { get; }
    public abstract bool Check(IEntity caster, TargetIntent intent);
}