using Godot;
using WowGd.Src.Input;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class BodyMover : Node, IMode
{
	[Export] private float _acceleration;
	[Export] private float _maxSpeed;
	[Export] private float _friction;

	private Body _body = null!;
	private IWishDir _wishDir = null!;

	public Vector2 CurrentWishDir {get; private set;}
	public Body Body => _body;

	public bool Enabled => _enabled;
	private bool _enabled = true;
	private bool _active  = false;

	public override void _Ready()
	{
		if (!this.TryGetSiblingComponent(out Body? body))
			return;

		if (!this.TryGetComponent(out IWishDir? wishDir))
			return;

		ModeRegistry.Register(Key.J, this);

		_body = body;
		_wishDir = wishDir;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;

		CurrentWishDir = _wishDir.WishDir();

		float currentSpeed = _body.Velocity.Dot(CurrentWishDir);
		float t = Mathf.Clamp(currentSpeed / _maxSpeed, 0f, 1f);
		float accelThisFrame = _acceleration * (1f - t) * dt; 
		_body.Velocity += accelThisFrame * CurrentWishDir;

		Vector2 drag = -_friction * _body.Velocity;

		if (CurrentWishDir != Vector2.Zero)
		{
			float communeDrag = Mathf.Max(0, drag.Dot(-CurrentWishDir));
			drag += communeDrag * CurrentWishDir;
		}

		_body.Velocity += drag * dt;
	}

	public bool Activate() =>
		DisableExt.IndempActivate(ref _active, _enabled, () => _wishDir.Enable());

	public bool Deactivate() =>
		DisableExt.IndempDeactivate(ref _active, _enabled, () => _wishDir.Disable());

	public bool Enable() =>
		DisableExt.IndempEnableActivable(ref _enabled, _active, () => _wishDir.Enable());

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => _wishDir.Disable());
}
