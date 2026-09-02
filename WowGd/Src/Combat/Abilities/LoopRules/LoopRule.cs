using Godot;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Data;

namespace WowGd.Src.Combat.Abilities.LoopRules;

[GlobalClass]
public abstract partial class LoopRule : Resource, ILoopRule
{
    public abstract string Id { get; }
    public abstract bool Check(ActuatePayload payload);
}