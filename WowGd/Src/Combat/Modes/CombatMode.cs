using Godot;
using WowGd.Src.Entities.Modes;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Modes;

[GlobalClass]
public abstract partial class CombatMode : Node, IMode
{
    [Export] private Key _key;
    public bool Enabled => _enabled;
    private bool _enabled = true;
    public bool Active => _active;
	private bool _active  = false;

    public override void _Ready()
    {
        if (PreReadySpec())
		    ModeRegistry.Register(_key, this);
    }

    protected abstract bool PreReadySpec();
    protected abstract void ActivateSpec();
    protected abstract void DeactivateSpec();
    protected abstract void DisableSpec();
    protected abstract void EnableSpec();

    public bool Activate() =>
		DisableExt.IndempActivate(ref _active, _enabled, ActivateSpec);

	public bool Deactivate() =>
		DisableExt.IndempDeactivate(ref _active, _enabled, DeactivateSpec);

	public bool Enable() =>
		DisableExt.IndempEnableActivable(ref _enabled, _active, ActivateSpec);

	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, DeactivateSpec);
}