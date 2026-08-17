
using System;
using Godot;
using WowGd.Src.Input.Generators;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Control;

public abstract partial class CursorMover<T> : Node, IDisablable
where
    T: Node, IVec2InputGenerator, new()
{
    protected T _generator = null!;
    protected Cursor _cursor = null!;
    private readonly ProcessToggle _toggle = new();

    public override sealed void _Ready()
    {
        if (!this.TryGetComposed(out Cursor? cursor))
            return;
        
        _cursor = cursor;
        _toggle.Toggled += SetPhysicsProcess;
        _cursor.Toggled += OnCursorToggled;

        _generator = this.CreateComponent<T>();
    }

    private void OnCursorToggled(bool enabled)
    {
        if (enabled)
            Enable();
        else
            Disable();
    }

    public bool Disable()
    {
        _generator.Disable();
        return _toggle.Disable();
    }

    public bool Enable()
    {
        _generator.Enable();
        return _toggle.Enable();
    }
}