using Godot;

namespace WowGd.Src.Physics.Movement.Data;

/// <summary>
/// A simple data structure that represents a movement components.
/// </summary>
/// <param name="target">
/// The target of the movement.
/// </param>
/// <param name="maxSpeed">
/// The maximum speed the moved object should reach.
/// </param>
/// <param name="acceleration">
/// The maximum acceleration the object can reach.
/// </param>
/// <param name="deceleration">
/// The maximum deceleration the object can reach.
/// This can be ambiguous as deceleration is, physically speaking, just acceleration.
/// It is used to create more responsive feeling.
/// The exact physical implication of this distinction is up to the logic system that uses this data.
/// </param>
public struct TargetMove(
    Vector2 target,
    float maxSpeed = 0f,
    float acceleration = 0f,
    float deceleration = 0f)
{
    public Vector2  Target          = target;
    public float    MaxSpeed        = maxSpeed;
    public float    Acceleration    = acceleration;
    public float    Deceleration    = deceleration;
}