using Godot;

namespace WowGd.Src.Tools;

public static class Vector3Ext
{
    public static Vector2 ToVector2(this Vector3 position) =>
        new(position.X, position.Z);
}