using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Control;

public abstract partial class CursorMoverBase : Node, IDisablable
{
    public abstract bool Enabled { get; }
    public abstract bool Disable();
    public abstract bool Enable();
}