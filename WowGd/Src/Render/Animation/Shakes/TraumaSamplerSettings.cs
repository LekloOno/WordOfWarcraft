using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public partial class TraumaSamplerSettings : Resource, ITraumaSamplerSettings
{
    [Export] public Noise Noise         { get; private set; } = null!;
    [Export] public float NoiseSpeed    { get; private set; }
}