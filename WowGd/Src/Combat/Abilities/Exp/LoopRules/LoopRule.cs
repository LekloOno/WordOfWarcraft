using Godot;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Data;

namespace WowGd.Src.Combat.Abilities.Exp.LoopRules;

[GlobalClass]
public abstract partial class LoopRule : Resource, ILoopRule
{
    public abstract string Id { get; }
    public abstract bool Check(ActuatePayload payload);
}