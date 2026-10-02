using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

[GlobalClass]
public partial class BodyMover : InternalContributor
{
	[Export] private float _acceleration;
	[Export] private bool _ground = true;

	private IEntity _entity = null!;
	public IBody Body => _entity.Body;

	public Vector2 CurrentWishDir {get; private set;}

	public bool Enabled => _enabled;

    public override DynamicTackleNode DynamicTackleNode => _entity.EntityMover.DynamicTackleNode;

    private bool _enabled = false;

	public override async void _Ready()
	{
		if (!this.TryGetComposedRecursive(out IEntity? entity))
			return;

		_entity = entity;
		
		InternalPriority priority = InternalPriorityExt.From(_ground, 0);

		await _entity.Initialization;
		_entity.EntityMover.AddContributor(MovementChannels.Internal, this, (uint)priority);
	}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => {});

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => {});

	protected override void GetForces(InternalLayerInput input, float delta, out Vector2? accel, out Vector2? raw)
	{
		CurrentWishDir = _enabled ? input.WishDir : Vector2.Zero;

		float currentSpeed = Body.Inertia.Dot(CurrentWishDir);
		float t = Mathf.Clamp(currentSpeed / input.MaxSpeed, 0f, 1f);
		float accelThisFrame = _acceleration * (1f - t);
		
		accel = accelThisFrame * CurrentWishDir * delta;
		raw = null;
	}

	public override void OnChannelClosed() { }
	public override void OnChannelOpened() { }
}
