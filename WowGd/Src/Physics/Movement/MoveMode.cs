using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class MoveMode : Node
{
	private BodyMover _mover = null!;

	public bool Enabled => _enabled;
	private bool _enabled = true;
	private bool _active  = false;

	public override void _Ready()
	{
		if (!this.TryGetComposedRecursive(out IEntity? entity))
			return;

		if (entity is not Node entityNode)
			return;

		if (!entityNode.TryGetComponent(out _mover!))
			return;

		_active = true;
		_mover.Enable();
	}

	public bool Enable() =>
		DisableExt.IndempEnableActivable(ref _enabled, _active, () => _mover.Enable());

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => _mover.Disable());

    public bool Activate() =>
		DisableExt.IndempActivate(ref _active, _enabled, () => _mover.Enable());
    public bool Deactivate() =>
		DisableExt.IndempDeactivate(ref _active, _enabled, () => _mover.Disable());
}
