using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Debug;

public partial class CollisionShapeBuilder : Node
{
    [Export] private Node2D _physics2d = null!;

    public override void _Ready()
    {
        BuildFrom(this);
    }

    private void BuildFrom(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is not CollisionShape3D col)
            {
                BuildFrom(child);
                continue;   
            }

            if (!TryConvert(col.Shape, out Shape2D? shape2d))
                continue;

            CollisionShape2D col2d = new () { Shape = shape2d };
            StaticBody2D body2d = new();

            _physics2d.AddChild(body2d);

            body2d.AddChild(col2d);

            body2d.GlobalPosition = col.GlobalPosition.ToVector2() + Vector2.Up;
            body2d.GlobalRotation = -col.GlobalRotation.Y;
        }
    }

    private static bool TryConvert(Shape3D shape3d, [NotNullWhen(true)] out Shape2D? shape2d)
    {
        if (shape3d is BoxShape3D box)
        {
            shape2d = new RectangleShape2D() { Size = box.Size.ToVector2() };
            return true;
        }

        if (shape3d is SphereShape3D sphere)
        {
            shape2d = new CircleShape2D() { Radius = sphere.Radius };
            return true;
        }

        if (shape3d is CylinderShape3D cylinder)
        {
            shape2d = new CircleShape2D() { Radius = cylinder.Radius };
            return true;
        }

        shape2d = null;
        return false;
    }
}