using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

[GlobalClass]
public partial class TraumaSettings : Resource, ITraumaSettings
{
    [Export] public float DecayRate     { get; private set; } = 1f;
    [Export] public float Cap           { get; private set; } = 10f;
}