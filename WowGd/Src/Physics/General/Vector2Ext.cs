using Godot;

namespace WowGd.Src.Physics.General;

public static class Vector2Ext
{
    public static Vector3 ToVector3(this Vector2 position, float y = 0f) =>
        new(position.X, y, position.Y);
}