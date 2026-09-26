using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.Internal;
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

	public void Lock()
	{
		GD.Print("we be lockin");
	}

	public void Dodge()
	{
		GD.Print("we be dodgin");
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
