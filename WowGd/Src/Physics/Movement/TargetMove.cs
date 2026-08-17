using Godot;

namespace WowGd.Src.Physics.Movement.Data;

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