using Godot;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class BodyMover : Node
{
	[Export] private float _acceleration;
	[Export] private float _maxSpeed;
	[Export] private float _friction;

	private IBody _body = null!;
	private IWishDir _wishDir = null!;

	public Vector2 CurrentWishDir {get; private set;}
	public IBody Body => _body;

	public bool Enabled => _enabled;
	private bool _enabled = false;

	public override void _Ready()
	{
		if (!this.TryGetSiblingComponent(out IBody? body))
			return;

		if (!this.TryGetComponent(out IWishDir? wishDir))
			return;

		_body = body;
		_wishDir = wishDir;
		_wishDir.Disable();
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentWishDir = _wishDir.WishDir();

		float currentSpeed = _body.LinearVelocity.Dot(CurrentWishDir);
		float t = Mathf.Clamp(currentSpeed / _maxSpeed, 0f, 1f);
		float accelThisFrame = _acceleration * (1f - t); 
		_body.ApplyForce(accelThisFrame * CurrentWishDir);

		Vector2 drag = -_friction * _body.LinearVelocity;

		if (CurrentWishDir != Vector2.Zero)
		{
			float communeDrag = Mathf.Max(0, drag.Dot(-CurrentWishDir));
			drag += communeDrag * CurrentWishDir;
		}

		_body.ApplyForce(drag);
	}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => _wishDir.Enable());

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => _wishDir.Disable());
}
