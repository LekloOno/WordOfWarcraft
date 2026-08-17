using Godot;

namespace WowGd.Src.Input.Debug;

[GlobalClass]
public partial class RandomMode : Node, IMode
{
    public override void _Ready()
    {
        ModeRegistry.Register(Key.Z, this);
    }

    public bool Disable() => true;
    public bool Enable() => true;
}