using System.Collections.Generic;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Render.WorldRenderer;

[GlobalClass]
public partial class WorldRenderer3D : Node3D
{
    private readonly Dictionary<IEntity, EntityRender3D> _entities = [];

    public override void _EnterTree()
    {
        EntitiesRegistry.EntityCreated += OnEntityCreated;
        EntitiesRegistry.EntityDeleted += OnEntityDeleted;
    }

    public override void _ExitTree()
    {
        EntitiesRegistry.EntityCreated -= OnEntityCreated;
        EntitiesRegistry.EntityDeleted -= OnEntityDeleted;
    }

    private void OnEntityDeleted(IEntity entity)
    {
        if (!_entities.TryGetValue(entity, out EntityRender3D? render3D))
            return;

        render3D.QueueFree();
        _entities.Remove(entity);
    }

    private void OnEntityCreated(IEntity entity)
    {
        if (entity.TryBuildRender3D(out EntityRender3D? render3d))
        {
            _entities.Add(entity, render3d);
            AddChild(render3d);
        }
    }
}