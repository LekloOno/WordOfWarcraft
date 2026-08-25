using Godot;
using WowGd.Src.Input.Generators;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Control;

public abstract partial class CursorMover<T> : CursorMoverBase
where
    T: Node, IVec2InputGenerator, new()
{
    public CursorMover() {}
    public CursorMover(Cursor cursor)
    {
        _cursor = cursor;
    }

    protected T _generator = null!;
    protected Cursor _cursor = null!;
    private readonly ProcessToggle _toggle = new();

    public override sealed void _Ready()
    {
        if (_cursor == null)
        {
            if (!this.TryGetComposed(out Cursor? cursor))
                return;
            _cursor = cursor;
        }
        
        _toggle.Toggled += SetPhysicsProcess;
        _cursor.Toggled += OnCursorToggled;

        _generator = this.CreateComponent<T>();
    }

    public sealed override void _ExitTree()
    {
        _toggle.Toggled -= SetPhysicsProcess;
        _cursor.Toggled -= OnCursorToggled;
    }

    private void OnCursorToggled(bool enabled)
    {
        if (enabled)
            Enable();
        else
            Disable();
    }

    public override bool Disable()
    {
        _generator.Disable();
        return _toggle.Disable();
    }

    public override bool Enable()
    {
        _generator.Enable();
        return _toggle.Enable();
    }

    public override bool Enabled => _toggle.Enabled;
}