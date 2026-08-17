using System.Collections.Generic;
using Godot;

namespace WowGd.Src.Input.Targeting.Free;

public static class GridKey
{
    private static readonly Dictionary<Key, Vector2> _inputsMap = [];
    private static readonly Dictionary<Vector2, Key> _keysMap = [];

    public static void Register(Key key, Vector2 position)
    {
        _inputsMap[key] = position;
        _keysMap[position] = key;
    }

    public static bool TryGetPosition(Key key, out Vector2 position) =>
        _inputsMap.TryGetValue(key, out position);

    public static Vector2 Position(Key key) =>
        _inputsMap.GetValueOrDefault(key, Vector2.Zero);

    // Mostly for UI
    public static bool TryGetKey(Vector2 position, out Key key) =>
        _keysMap.TryGetValue(position, out key);
}