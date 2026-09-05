using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Targeting;
using WowGd.Src.Tools;

namespace WowGd.Src.Render.WorldRenderer;

public partial class EntityRender3D : Node3D, IEntityRenderInitializer
{
    [Export] private DirectTargetUi3D _directTargetUi = null!;
    public IEntity Entity = null!;

    public EntityRender3D() { }
    public EntityRender3D(IEntity entity) { Entity = entity; }

    public override void _PhysicsProcess(double delta)
    {
        Position = Entity.Body.GlobalPosition.ToVector3();
    }

    public bool Init(IEntity entity, [NotNullWhen(true)] out EntityRender3D? renderer)
    {
        Entity = entity;

        if (_directTargetUi != null)
            _directTargetUi.Entity = entity;

        renderer = this;
        return InitSpec(entity);

    }

    public virtual bool InitSpec(IEntity entity) { return true; }

}
