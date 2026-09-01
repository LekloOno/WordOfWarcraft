using Godot;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;

[GlobalClass]
public partial class InstantTriggerData : TriggerData
{
    public override string Id => "trigger_instant";

    public override ITrigger Build() =>
        new InstantTrigger();
}