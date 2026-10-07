using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public static class TraumaShakeSampling
{
    public static Vector3 Sample3D(Noise n, float intensity, float time, float speed, int seed = 1) => new(
        intensity * n.GetNoise2D(time * speed, seed * 1000f + 0f),
        intensity * n.GetNoise2D(time * speed, seed * 1000f + 100f),
        intensity * n.GetNoise2D(time * speed, seed * 1000f + 200f));

    
    public static Vector2 Sample2D(Noise n, float intensity, float time, float speed, int seed = 1) => new(
        intensity * n.GetNoise2D(time * speed, seed * 1000f + 0f),
        intensity * n.GetNoise2D(time * speed, seed * 1000f + 100f));

    public static Vector3 Sample3D(TraumaChannel channel, float time, int seed = 1) =>
        Sample3D(channel.SamplerSettings.Noise, channel.Intensity, time, channel.SamplerSettings.NoiseSpeed, seed);

    public static Vector2 Sample2D(TraumaChannel channel, float time, int seed = 1) =>
        Sample2D(channel.SamplerSettings.Noise, channel.Intensity, time, channel.SamplerSettings.NoiseSpeed, seed);
}