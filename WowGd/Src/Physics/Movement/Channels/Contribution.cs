using Godot;

namespace WowGd.Src.Physics.Movement.Channels;

public readonly struct Contribution(
    Vector2? acceleration = null,
    Vector2? rawVelocity = null,
    float frictionRatio = 1f
) {
    public readonly Vector2 Acceleration = acceleration ?? Vector2.Zero;
    public readonly Vector2 RawVelocity  = rawVelocity ?? Vector2.Zero;
    public readonly float FrictionRatio  = frictionRatio;

    public Contribution() : this(null, null, 1f) {}

    public readonly Contribution Add(Contribution b) =>
        new(Acceleration    + b.Acceleration,
            RawVelocity     + b.RawVelocity,
            FrictionRatio   * b.FrictionRatio);

    public static Contribution operator +(Contribution a, Contribution b) =>
        a.Add(b);
}