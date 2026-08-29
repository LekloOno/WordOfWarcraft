using Godot;
using WowGd.Src.Dactylo.Abilities;
using WowGd.Src.Entities.Modes;
using WowGd.Src.Tools;

namespace WowGd.Src.Debug;

[GlobalClass]
public partial class RandomMode : Node, IMode
{
    [Export] private WordBasedAbility _ability = null!;

    public bool Enabled => _enabled;
    private bool _enabled = true;
    private bool _active = false;

    public override void _Ready()
    {
        ModeRegistry.Register(Key.K, this);
    }

    public bool Activate() =>
        DisableExt.IndempActivate(ref _active, _enabled, () => _ability.Enable());

    public bool Deactivate() =>
        DisableExt.IndempDeactivate(ref _active, _enabled, () => _ability.Disable());

    public bool Enable() =>
        DisableExt.IndempEnableActivable(ref _enabled, _active, () => _ability.Enable());

    public bool Disable() =>
        DisableExt.IndempDisable(ref _enabled, () => _ability.Disable());
}