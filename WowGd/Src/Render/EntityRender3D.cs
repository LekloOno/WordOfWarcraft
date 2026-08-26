using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Render;
public partial class EntityRender3D : Node3D
{
    public EntityRender3D() {}
    public EntityRender3D(IEntity entity) { Entity = entity; }

    public IEntity Entity = null!;

    public override void _PhysicsProcess(double delta)
    {
        Position = Entity.Body.GlobalPosition.ToVector3();
    }

    public virtual void Init(IEntity entity)
    {
        Entity = entity;
    }
}