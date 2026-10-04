using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Render.WorldRenderer;

public static class EntityIdDataExt3D
{
    // private readonly static Texture2D _dudeText = GD.Load<Texture2D>("res://assets/sprites/entities/dude.png");
    private const string SimpleDudePath = "res://prefabs/render/player_template3d.tscn";
    private const string SimpleGoblinPath = "res://prefabs/render/goblin_template3d.tscn";
    private const string WitcherPillarPath = "res://prefabs/render/witcherpillar_template3d.tscn";
    private readonly static PackedScene _simpleDudeScene = GD.Load<PackedScene>(SimpleDudePath);
    private readonly static PackedScene _simpleGoblinScene = GD.Load<PackedScene>(SimpleGoblinPath);
    private readonly static PackedScene _witcherPillarScene = GD.Load<PackedScene>(WitcherPillarPath);

    public static bool TryBuildRender3D(this IEntity self, [NotNullWhen(true)] out EntityRender3D? render3d)
    {
        if (self.IdData.Key == "goblin")
        {
            IEntityRenderInitializer initializer = _simpleGoblinScene.Instantiate<IEntityRenderInitializer>();

            if (!initializer.Init(self, out render3d))
                return false;

            //render3d.Scale = Vector3.One * 1.6f;
            return true;
        }
        else if (self.IdData.Key == "player")
        {
            IEntityRenderInitializer initializer = _simpleDudeScene.Instantiate<IEntityRenderInitializer>();

            if (!initializer.Init(self, out render3d))
                return false;

            //render3d.Scale = Vector3.One * 1.7f;
            return true;
        } else if (self.IdData.Key == "witcherpillar")
        {
            IEntityRenderInitializer initializer = _witcherPillarScene.Instantiate<IEntityRenderInitializer>();

            if (!initializer.Init(self, out render3d))
                return false;
            return true;
        }

        render3d = null;
        return false;
    }
}
