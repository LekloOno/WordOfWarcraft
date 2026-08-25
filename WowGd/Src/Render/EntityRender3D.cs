using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Physics;
using WowGd.Src.Tools;

namespace WowGd.Src.Render;
public partial class EntityRender3D(IEntity entity) : Node3D
{
    private readonly IEntity _entity = entity;

    public override void _PhysicsProcess(double delta)
    {
        Position = _entity.Body.Position.ToVector3();
    }
}