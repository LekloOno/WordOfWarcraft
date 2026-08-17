using System;
using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free;

[GlobalClass]
public partial class Cursor : Node2D, IDisablable
{
    public Vector2 Target { get; private set; }
    private readonly ProcessToggle _toggle = new();

    public event Action<bool> Toggled
    {
        add =>
            _toggle.Toggled += value;
        remove =>
            _toggle.Toggled -= value;
    }
    
    public override void _Ready()
    {
        Target = Position;
    }

    public void Confirm()
    {
        Target = Position;
    }

    public void Reset()
    {
        Position = Target;
    }

    public bool Enable()
    {
        ProcessMode = ProcessModeEnum.Inherit;   
        return _toggle.Enable();
    }

    public bool Disable()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        return _toggle.Disable();
    }

    public bool Enabled => ProcessMode == ProcessModeEnum.Inherit;
}