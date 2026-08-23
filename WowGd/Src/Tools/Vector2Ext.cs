using Godot;

namespace WowGd.Src.Tools;

public static class Vector2Ext
{
    public static Vector3 ToVector3(this Vector2 position, float y = 0f) =>
        new(position.X, y, position.Y);

    /// <summary>
    /// Performs a samplerate-independant lerp.
    /// </summary>
    /// <param name="vec3"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static Vector2 ProcessLerp(this Vector2 from, Vector2 to, float speed, float delta)
    {
        float paramT = 1f - Mathf.Exp(-speed * (float) delta);
        return from.Lerp(to, paramT);
    }
}