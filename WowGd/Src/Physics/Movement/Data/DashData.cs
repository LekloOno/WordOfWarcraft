using Godot;

namespace WowGd.Src.Physics.Movement.Data;

[GlobalClass]
public partial class DashData : RunData
{
    /// <summary>
    /// The max distance for a dash to occur in meters.
    /// </summary>
    [Export] public float MaxDistance   { get; private set; } = 1.4f;

    /// <summary>
    /// The amount of time before the entity can start moving again after a dash.
    /// </summary>
    [Export] public float StunDuration      { get; private set; } = 0.5f;
}