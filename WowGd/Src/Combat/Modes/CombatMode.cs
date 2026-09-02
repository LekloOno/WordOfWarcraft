using Godot;

namespace WowGd.Src.Combat.Modes;

[GlobalClass]
public abstract partial class CombatMode : Node
{
    [Export] private Key _key;
    public bool Enabled => _enabled;
    private bool _enabled = true;
}