using Godot;

namespace WowGd.Src.Render.WorldRenderer;

[GlobalClass, Tool]
public partial class FixedSizeSprite3D : Sprite3D
{
    private float _height = 1.0f;

    [Export]
    public float Height
    {
        get => _height;
        set
        {
            if (value == _height) return;
            _height = value;
            UpdatePixelSize();
        }
    }

    public override void _Ready()
    {
        TextureChanged += UpdatePixelSize;
        UpdatePixelSize();
    }

    private void UpdatePixelSize()
    {
        float textureHeight = Texture.GetHeight();
        PixelSize = _height / textureHeight;
    }

}
