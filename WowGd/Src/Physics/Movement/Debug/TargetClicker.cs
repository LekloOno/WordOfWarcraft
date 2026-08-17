using Godot;
using WowGd.Src.Physics.General;

namespace WowGd.Src.Physics.Movement.Debug;

/// <summary>
/// Just a little debug tool to quickly test the movement physics.
/// 
/// The real game will not be based on clicks, it was just faster to implement and test.
/// </summary>
[GlobalClass]
public partial class TargetClicker : Node
{
    [Export] private Mover  _mover = null!;
    [Export] private Node3D _grid = null!;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
        {
            if (Tools.TryGetMouseWorldPosition(out Vector3 position, GetViewport()))
                _mover.RequestMoveTo((position - _grid.GlobalPosition).ToVector2());
        }
    }
}