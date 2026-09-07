using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class BodyMover : Node
{
	[Export] private float _acceleration;
	[Export] private float _maxSpeed;
	[Export] private float _friction;

	private IEntity _entity = null!;
	public IBody Body => _entity.Body;
	private IWishDir WishDir => _entity.WishDir;

	public Vector2 CurrentWishDir {get; private set;}

	public bool Enabled => _enabled;
	private bool _enabled = false;

	public override void _Ready()
	{
		if (!this.TryGetComposedRecursive(out IEntity? entity))
			return;

		_entity = entity;
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentWishDir = _enabled ? WishDir.WishDir() : Vector2.Zero;

		float currentSpeed = Body.LinearVelocity.Dot(CurrentWishDir);
		float t = Mathf.Clamp(currentSpeed / _maxSpeed, 0f, 1f);
		float accelThisFrame = _acceleration * (1f - t); 
		Body.ApplyForce(accelThisFrame * CurrentWishDir);

		Vector2 drag = -_friction * Body.LinearVelocity;

		if (CurrentWishDir != Vector2.Zero && currentSpeed <= _maxSpeed)
		{
			float communeDrag = Mathf.Max(0, drag.Dot(-CurrentWishDir));
			drag += communeDrag * CurrentWishDir;
		}

		Body.ApplyForce(drag);
	}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => {});

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => {});
}
