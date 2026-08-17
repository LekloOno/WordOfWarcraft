using Godot;
using WowGd.Src.Physics;
using WowGd.Src.Physics.General;

namespace WowGd.Src.Render;

/// <summary>
/// The game logic truly is just 2D.
/// I'd rather thus fully define it in 2D for now, and have pure presentation nodes to represents such in 3D.
/// Notably because we don't know exactly how we will present the game, in projected 3D, full 2D, isometric ...
/// 
/// Separating the presentation from the logic enables a high flexibility on that regard.
/// 
/// This nodes listens to the actual logic, and presents it in 3D, by binding it to an actual Node3D.
/// </summary>
[GlobalClass]
public partial class Body3D : Node3D
{
    [Export] private Body _body = null!;
    private Node3D _grid = null!;

    public override void _Ready()
    {
        if (GetParent() is not Node3D grid)
        {
            GD.PushError($"[{nameof(Body3D)}] requires a [{nameof(Node3D)}] parent as its grid.");
            return;
        }

        _grid = grid;
    }

    public override void _Process(double delta)
    {
        Position = _body.GetInterpollatedPosition().ToVector3() + _grid.GlobalPosition;
    }
}