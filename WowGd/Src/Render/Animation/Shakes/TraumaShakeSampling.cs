using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public static class TraumaShakeSampling
{
    public static Vector3 Sample3D(ShakeSpatialChannels spatialChannels, Noise n, float intensity, float time, float speed, int seed = 1) => new(
        spatialChannels.HasFlag(ShakeSpatialChannels.X) ? intensity * n.GetNoise2D(time * speed, seed * 1000f + 0f) : 0,
        spatialChannels.HasFlag(ShakeSpatialChannels.Y) ? intensity * n.GetNoise2D(time * speed, seed * 1000f + 100f) : 0,
        spatialChannels.HasFlag(ShakeSpatialChannels.Z) ? intensity * n.GetNoise2D(time * speed, seed * 1000f + 200f) : 0);

    public static Vector3 Sample3D(TraumaChannel channel, float time, int seed = 1) =>
        Sample3D(
            channel.SamplerSettings.SpatialChannels,
            channel.SamplerSettings.Noise,
            channel.Intensity,
            time,
            channel.SamplerSettings.NoiseSpeed,
            seed);
}