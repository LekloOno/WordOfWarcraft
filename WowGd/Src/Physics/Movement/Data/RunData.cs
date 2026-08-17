using Godot;

namespace WowGd.Src.Physics.Movement.Data;

[GlobalClass]
public partial class RunData : Resource
{
    /// <summary>
    /// Maximum speed reached in m/s.
    /// </summary>
    [Export] public float Speed         { get; private set; }

    /// <summary>
    /// Acceleration in m/s^2.
    /// </summary>
    [Export] public float Acceleration  { get; private set; }

    /// <summary>
    /// Deceleration in m/s^2
    /// </summary>
    [Export] public float Deceleration  { get; private set; }
}