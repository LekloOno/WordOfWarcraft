using Godot;

namespace WowGd.Src.Tools;

public static class Vector3Ext
{
    public static Vector2 ToVector2(this Vector3 position) =>
        new(position.X, position.Z);

    /// <summary>
    /// Performs a samplerate-independant lerp.
    /// </summary>
    /// <param name="vec3"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static Vector3 ProcessLerp(this Vector3 from, Vector3 to, float speed, float delta)
    {
        float paramT = 1f - Mathf.Exp(-speed * (float) delta);
        return from.Lerp(to, paramT);
    }
}