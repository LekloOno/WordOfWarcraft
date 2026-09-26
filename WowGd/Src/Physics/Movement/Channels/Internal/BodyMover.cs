using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

[GlobalClass]
public partial class BodyMover : InternalLayer
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
		
		InternalPriority priority = InternalPriority.Base |
			(_ground ? InternalPriority.Grounded : 0);

		_entity.EntityMover.AddContributor(MovementChannels.Internal, this, (uint)priority);
	}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => {});

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => {});

	protected override void GetForce(EntityMover mover, float delta, out Vector2? accel, out Vector2? raw)
	{
		CurrentWishDir = _enabled ? WishDir.WishDir() : Vector2.Zero;

		float currentSpeed = Body.Inertia.Dot(CurrentWishDir);
		float t = Mathf.Clamp(currentSpeed / MaxSpeed, 0f, 1f);
		float accelThisFrame = _acceleration * (1f - t);
		
		accel = accelThisFrame * CurrentWishDir * delta;
		raw = null;
	}

	public override void OnChannelClosed() { }
	public override void OnChannelOpened() { }
}
