using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.Internal;

[GlobalClass]
public partial class BodyMover : InternalMovementLayer
{
	[Export] private float _acceleration;
	[Export] private bool _ground = true;

	private IEntity _entity = null!;
	public IBody Body => _entity.Body;
	public override IWishDir WishDir => _entity.WishDir;

	public Vector2 CurrentWishDir {get; private set;}

	public bool Enabled => _enabled;
	private bool _enabled = false;

	public override void _Ready()
	{
		if (!this.TryGetComposedRecursive(out IEntity? entity))
			return;

		_entity = entity;
		
		if (_ground)
			_entity.EntityMover.Internal.GroundInternal.SetBase(this);
		else
			_entity.EntityMover.Internal.AirInternal.SetBase(this);
	}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => {});

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => {});

	protected override Vector2 GetForce(EntityMover mover, float delta)
	{
		CurrentWishDir = _enabled ? WishDir.WishDir() : Vector2.Zero;

		float currentSpeed = Body.LinearVelocity.Dot(CurrentWishDir);
		float t = Mathf.Clamp(currentSpeed / MaxSpeed, 0f, 1f);
		float accelThisFrame = _acceleration * (1f - t);
		Vector2 f = accelThisFrame * CurrentWishDir * delta; 
		return Body.LinearVelocity + f;
	}

	public override void OnChannelClosed() { }
	public override void OnChannelOpened() { }
}
