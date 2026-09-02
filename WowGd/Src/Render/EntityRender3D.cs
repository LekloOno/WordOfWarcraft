using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Targeting;
using WowGd.Src.Tools;

namespace WowGd.Src.Render;
public partial class EntityRender3D : Node3D
{
    [Export] private DirectTargetUi3D _directTargetUi = null!;
    public IEntity Entity = null!;

    public EntityRender3D() {}
    public EntityRender3D(IEntity entity) { Entity = entity; }

    public override void _PhysicsProcess(double delta)
    {
        Position = Entity.Body.GlobalPosition.ToVector3();
    }

    public void Init(IEntity entity)
    {
        Entity = entity;
        
        if (_directTargetUi != null)
            _directTargetUi.Entity = entity;

        InitSpec(entity);
    }

    public virtual void InitSpec(IEntity entity) {}
}