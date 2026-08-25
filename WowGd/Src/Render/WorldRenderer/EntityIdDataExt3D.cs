using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Render.WorldRenderer.Entities;

namespace WowGd.Src.Render.WorldRenderer;

public static class EntityIdDataExt3D
{
    private readonly static Texture2D _dudeText = GD.Load<Texture2D>("res://assets/sprites/entities/dude.png");
    private const string SimpleDudePath = "res://prefabs/dude_template3d.tscn";
    private readonly static PackedScene _simpleDudeScene = GD.Load<PackedScene>(SimpleDudePath);

    public static bool TryBuildRender3D(this IEntity self, [NotNullWhen(true)] out EntityRender3D? render3d)
    {
        if (self.IdData.Key == "goblin")
        {
            SimpleDude dude = _simpleDudeScene.Instantiate<SimpleDude>();
            dude.Init(self);
            dude.Scale = Vector3.One * 0.85f;
            render3d = dude;

            return true;
        }
        else if (self.IdData.Key == "player")
        {
            
            SimpleDude dude = _simpleDudeScene.Instantiate<SimpleDude>();
            dude.Init(self);
            dude.Scale = Vector3.One * 1.5f;
            render3d = dude;

            return true;
        }
        
        render3d = null;
        return false;
    }
}