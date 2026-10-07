using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

[GlobalClass]
public partial class TraumaSamplerSettings : Resource, ITraumaSamplerSettings
{
    [Export] public Noise Noise         { get; private set; } = null!;
    [Export] public float NoiseSpeed    { get; private set; } = 1f;
    [Export] public ShakeSpatialChannels SpatialChannels { get; private set; } =
        ShakeSpatialChannels.X | ShakeSpatialChannels.Y | ShakeSpatialChannels.Z;
}