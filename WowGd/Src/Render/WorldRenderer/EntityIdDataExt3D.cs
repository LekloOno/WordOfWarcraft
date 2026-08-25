using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Render.WorldRenderer;

public static class EntityIdDataExt3D
{
    private readonly static Texture2D _dudeText = GD.Load<Texture2D>("res://assets/sprites/entities/dude.png");

    public static bool TryBuildRender3D(this IEntity self, [NotNullWhen(true)] out EntityRender3D? render3d)
    {
        render3d = new(self);

        Sprite3D sprite = new()
        {
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
            Texture = _dudeText
        };

        render3d.AddChild(sprite);

        if (self.IdData.Key == "goblin")
        {
            Vector3 position = sprite.Position;
            position.Y = 0.3f;
            sprite.Position = position;

            sprite.Scale = Vector3.One * 0.85f;
        }
        else if (self.IdData.Key == "player")
        {
            Vector3 position = sprite.Position;
            position.Y = 0.5f;
            sprite.Position = position;

            sprite.Scale = Vector3.One * 1.5f;
        }
        else
            return false;

        
        GD.Print("oi");
        return true;
    }
}