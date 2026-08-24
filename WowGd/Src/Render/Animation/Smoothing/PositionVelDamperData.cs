using Godot;

namespace WowGd.Src.Render.Animation.Smoothing;

[GlobalClass]
public partial class PositionVelDamperData : Resource
{
    [Export] public float Speed         { get; private set; } = 3f;
    [Export] public float VelDamping    { get; private set; } = 8f;
}