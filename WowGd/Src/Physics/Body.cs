using Godot;

namespace WowGd.Src.Physics;

/// <summary>
/// Holds and computes the physics of a targeteable body.
///
/// That is a body that can be assigned to reach a given destination.
///
/// It's not deriving Node2D nor Node3D, as it is a pure logic Node.
/// Representation should be handled separetely, as we're still in early prototyping phase,
/// and don't have a fixed view on what we want the game to look like.
/// Fully separating the logic can only be a good thing anyways.
/// </summary>
[GlobalClass]
public partial class Body : CharacterBody2D, IBody
{
	public Vector2 RawVelocity  { get; private set; }
	public Vector2 Inertia      { get; private set; }

	private Vector2 _rawVelocityBuffer;

	public void AddRawForce(Vector2 raw) =>
		_rawVelocityBuffer += raw;

	public void Accelerate(Vector2 accel) =>
		Inertia += accel;

	public override void _Ready()
	{
		MotionMode = MotionModeEnum.Floating;
	}

	public override void _PhysicsProcess(double delta)
	{
		RawVelocity = _rawVelocityBuffer;
		_rawVelocityBuffer = Vector2.Zero;

		Velocity = Inertia + RawVelocity;
		MoveAndSlide();
	}
}
