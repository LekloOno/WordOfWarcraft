using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Modes;

[GlobalClass]
public abstract partial class CombatMode : Node, IDisablable
{
    [Export] private Key _key;
    public bool Enabled => _enabled;
    private bool _enabled = false;

    public bool Enable() =>
        DisableExt.IndempEnable(ref _enabled, EnableSpec);
    public bool Disable() =>
        DisableExt.IndempDisable(ref _enabled, DisableSpec);
    protected abstract void EnableSpec();
    protected abstract void DisableSpec();
}