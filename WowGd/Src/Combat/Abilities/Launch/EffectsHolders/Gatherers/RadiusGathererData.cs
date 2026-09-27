using Godot;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;

[GlobalClass]
public partial class RadiusGathererData : GathererData
{
    public override string Id => "gatherer_radius";
    [Export] public float Radius { get; private set; } = 2f;
    [Export] public RadiusGathererSettings Settings { get; private set; } = 
        RadiusGathererSettings.WeightOnDistance | RadiusGathererSettings.IncludeDirect;

    public override IGatherer Build() =>
        new RadiusGatherer(this);
}