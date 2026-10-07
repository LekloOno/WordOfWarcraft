using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public interface ITraumaSamplerSettings
{
    Noise Noise         { get; }
    float NoiseSpeed    { get; }
}