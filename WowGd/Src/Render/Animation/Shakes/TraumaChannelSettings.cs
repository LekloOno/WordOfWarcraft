using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public partial class TraumaChannelSettings : Resource, ITraumaSettings, ITraumaSamplerSettings
{
    [Export] private TraumaSettings _traumaSettings = null!;
    [Export] private TraumaSamplerSettings _traumaSamplerSettings = null!;

    public float DecayRate => _traumaSettings.DecayRate;
    public float Cap => _traumaSettings.Cap;
    public Noise Noise => _traumaSamplerSettings.Noise;
    public float NoiseSpeed => _traumaSamplerSettings.NoiseSpeed;
}