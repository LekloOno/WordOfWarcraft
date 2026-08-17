using Godot;
using WowGd.Src.Input.Generators;

namespace WowGd.Src.Physics.Movement.WishDir;

[GlobalClass]
public partial class StandardWishDir : Node, IWishDir
{
    private StandardKeyGenerator _generator = new();

    public override void _Ready()
    {
        AddChild(_generator);
    }

    public Vector2 WishDir()
    {
        _generator.Retrieve(out Vector2 wishDir);
        return wishDir;
    }

    public bool Disable()
    {
        return _generator.Disable();
    }

    public bool Enable()
    {
        return _generator.Enable();
    }

    public bool Enabled => _generator.Enabled;
}