using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public partial class TraumaSettings : Resource, ITraumaSettings
{
    [Export] public float DecayRate     { get; private set; }
    [Export] public float Cap           { get; private set; }
}