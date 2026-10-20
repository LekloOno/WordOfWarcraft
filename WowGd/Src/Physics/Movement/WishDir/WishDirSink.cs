using Godot;

namespace WowGd.Src.Physics.Movement.WishDir;

public class WishDirSink : IWishDir
{
    public readonly static WishDirSink Sink = new();

    public bool Enabled => false;

    public bool Disable() => true;
    public bool Enable() => true;

    public Vector2 WishDir() => Vector2.Zero;
}