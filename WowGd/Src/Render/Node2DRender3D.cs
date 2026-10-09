using System;
using Godot;
using WowGd.Src.Tools;

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
public partial class Node2DRender3D : Node3D
{
    public Node2DRender3D() { GridPosition = () => Vector3.Zero; }
    public Node2DRender3D(Node2D node) : this() { _node = node; }

    [Export] private Node2D _node = null!;
    [Export] private bool _gridRelative = false;
    private Node3D _grid = null!;

    private ProcessType _processType = ProcessType.PhysicsProcess;
    [Export] private ProcessType ProcessType
    {
        get => _processType;
        set
        {
            if (value == _processType)
                return;

            _processType = value;
            UpdateProcessType();
        }
    }

    private void UpdateProcessType()
    {
        if (_processType == ProcessType.Process)
        {
            PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
            SetPhysicsProcess(false);
            SetProcess(true);
        }
        else
        {
            PhysicsInterpolationMode = PhysicsInterpolationModeEnum.On;
            SetPhysicsProcess(true);
            SetProcess(false);
        }
    }

    public override void _Ready()
    {
        if (this.TryGetComposed(out Node3D? grid))
            _grid = grid;

        if (!_gridRelative || _grid is null)
            GridPosition = () => Vector3.Zero;
        else
            GridPosition = () => _grid.GlobalPosition;

        UpdateProcessType();
    }

    private Func<Vector3> GridPosition;

    public override void _PhysicsProcess(double delta)
    {
        Position = _node.Position.ToVector3() + GridPosition();
    }

    public override void _Process(double delta)
    {
        Position = _node.Position.ToVector3() + GridPosition();
    }
}

public enum ProcessType
{
    Process,
    PhysicsProcess,
}