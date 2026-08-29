using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data.Models;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.CasterRules;

[GlobalClass]
public abstract partial class CasterRule : Resource, ICasterRule
{
    public abstract string Id { get; }
    public abstract bool Check(IEntity caster);
}