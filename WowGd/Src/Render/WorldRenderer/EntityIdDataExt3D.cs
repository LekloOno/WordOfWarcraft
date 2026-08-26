using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Render.WorldRenderer.Entities;

namespace WowGd.Src.Render.WorldRenderer;

public static class EntityIdDataExt3D
{
    // private readonly static Texture2D _dudeText = GD.Load<Texture2D>("res://assets/sprites/entities/dude.png");
    private const string SimpleDudePath = "res://prefabs/render/dude_template3d.tscn";
    private const string SimpleGoblinPath = "res://prefabs/render/goblin_template3d.tscn";
    private readonly static PackedScene _simpleDudeScene = GD.Load<PackedScene>(SimpleDudePath);
    private readonly static PackedScene _simpleGoblinScene = GD.Load<PackedScene>(SimpleGoblinPath);

    public static bool TryBuildRender3D(this IEntity self, [NotNullWhen(true)] out EntityRender3D? render3d)
    {
        if (self.IdData.Key == "goblin")
        {
            EntityRender3D renderer = _simpleGoblinScene.Instantiate<SimpleGoblin>();
            renderer.Init(self);
            renderer.Scale = Vector3.One * 1.2f;
            render3d = renderer;

            return true;
        }
        else if (self.IdData.Key == "player")
        {

            EntityRender3D renderer = _simpleDudeScene.Instantiate<SimpleDude>();
            renderer.Init(self);
            renderer.Scale = Vector3.One * 1.5f;
            render3d = renderer;

            return true;
        }

        render3d = null;
        return false;
    }
}
