using Godot;
using WowGd.Src.Input;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class MoveMode : Node, IMode
{
	private BodyMover _mover = null!;

	public bool Enabled => _enabled;
	private bool _enabled = true;
	private bool _active  = false;

	public override void _Ready()
	{
		if (!this.TryGetSiblingComponent(out BodyMover? mover))
            return;

		ModeRegistry.Register(Key.J, this);
        _mover = mover;
	}

	public bool Activate() =>
		DisableExt.IndempActivate(ref _active, _enabled, () => _mover.Enable());

	public bool Deactivate() =>
		DisableExt.IndempDeactivate(ref _active, _enabled, () => _mover.Disable());

	public bool Enable() =>
		DisableExt.IndempEnableActivable(ref _enabled, _active, () => _mover.Enable());

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => _mover.Disable());
}
