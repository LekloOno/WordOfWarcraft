using Godot;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;

[GlobalClass]
public partial class DirectGathererData : GathererData
{
    public override string Id => "gatherer_direct";
    public override IGatherer Build() =>
        new DirectGatherer();
}