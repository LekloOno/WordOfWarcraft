using System.Collections.Generic;
using System.Threading;
using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Combat.Abilities.Targeting.TargetRules;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Stats.Tackle;
using WowGd.Src.Physics.Movement.Channels.Internal;
using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;
using WowGd.Src.Physics.Movement.Status;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class MoveMode : Node
{
	private BodyMover _mover = null!;
	public IAbility? MovementAbility { get; private set; }
	private IEntity _entity = null!;

	public bool Enabled => _enabled;
	private bool _enabled = true;
	private bool _active  = false;

	public override void _Ready()
	{
		if (!this.TryGetComposedRecursive(out IEntity? entity))
			return;

		_entity = entity;

		if (entity is not Node entityNode)
			return;

		if (!entityNode.TryGetComponent(out _mover!))
			return;

		if (this.TryGetComponent(out IAbility? movementAbility))
			MovementAbility = movementAbility;

		_active = true;
		DoEnable();
	}

	public void StartMovementAbility()
	{
		MovementAbility?.Start(_entity);
	}

	private CancellationTokenSource? _cts;
	private readonly IEnumerable<ITargetRule> _tackleTargetRules = [new RelationTargetRule(TargetRelation.Enemy)];
	public async void Tackle()
	{
		IEntityMover entityMover = _entity.EntityMover;
		DynamicTackleNode tackleNode = entityMover.DynamicTackleNode;

		if (tackleNode.IsTackling)
		{
			tackleNode.ReleaseTackle();
			return;
		}

		if (!entityMover.StatusChannels.State.CanTackle())
			return;

		_cts?.Cancel();
		_cts = new();

		TargetResult result = await _entity.TargetIntentDriver.RetrieveTargetIntent(
			_entity, TargetIntentAcquirer.Direct, _cts.Token, _tackleTargetRules);

		if (!result.TryGet(out TargetIntent intent))
			return;

		tackleNode.StartTackle(intent.Entity!);
	}

	public void CancelTackleTargeting() =>
		_cts?.Cancel();

	public void Dodge()
	{
		IEntityMover entityMover = _entity.EntityMover;
		DynamicTackleNode tackleNode = entityMover.DynamicTackleNode;

		if (!tackleNode.IsTackled)
			return;

		tackleNode.DrainTacklers(0.2);
	}

	public bool Enable() =>
		DisableExt.IndempEnableActivable(ref _enabled, _active, DoEnable);

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, DoDisable);

    public bool Activate() =>
		DisableExt.IndempActivate(ref _active, _enabled, DoEnable);
    public bool Deactivate() =>
		DisableExt.IndempDeactivate(ref _active, _enabled, DoDisable);


	private void DoEnable()
	{
		_mover.Enable();
		MovementAbility?.Enable();
	}

	private void DoDisable()
	{
		_mover.Disable();
		MovementAbility?.Disable();
	}
}
