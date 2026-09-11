using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Generators;

[GlobalClass]
public partial class StandardKeyGenerator : Node, IVec2InputGenerator
{
    public const string Up      = "move_up";
    public const string Down    = "move_down";
    public const string Left    = "move_left";
    public const string Right   = "move_right";

    private Vector2 _walkAxis = Vector2.Zero;

    private readonly ProcessToggle _toggle = new();

    public override void _Ready()
    {
        _toggle.Toggled += SetProcess;
    }

    public override void _Process(double delta)
    {
        _walkAxis = Godot.Input.GetVector(Left, Right, Up, Down);
    }

    public bool Retrieve(out Vector2 vec)
    {
        if (_toggle.Enabled)
            vec = _walkAxis;
        else
            vec = Vector2.Zero;

        return true;
    }

    public bool Enable()  => _toggle.Enable();
    public bool Disable() => _toggle.Disable();
    public bool Enabled => _toggle.Enabled;
    
}