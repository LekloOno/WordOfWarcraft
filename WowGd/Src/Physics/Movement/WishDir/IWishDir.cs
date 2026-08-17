using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.WishDir;

public interface IWishDir : IDisablable
{
    public Vector2 WishDir();
}