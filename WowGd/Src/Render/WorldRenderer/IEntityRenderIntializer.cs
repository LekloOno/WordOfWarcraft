using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Render.WorldRenderer;

public interface IEntityRenderInitializer
{
    bool Init(IEntity entity, [NotNullWhen(true)] out EntityRender3D? renderer);
}
