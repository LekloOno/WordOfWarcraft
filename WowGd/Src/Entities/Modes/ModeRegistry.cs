using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace WowGd.Src.Entities.Modes;

public static class ModeRegistry
{
    public static KeyModifierMask Modifier = KeyModifierMask.MaskCtrl;
    private static readonly Dictionary<Key, IMode> _modesMap = [];

    public static void Register(Key key, IMode mode) =>
        _modesMap.Add(key, mode);

    public static void Unregister(Key key) =>
        _modesMap.Remove(key);

    public static bool TryGetMode(InputEvent @event, [NotNullWhen(true)] out IMode? mode)
    {
        mode = null;

        if (@event is not InputEventKey keyEvent)
            return false;

        if ((keyEvent.GetModifiersMask() & Modifier) == 0)
            return false;

        if (keyEvent.Echo || !keyEvent.Pressed)
            return false;

        return _modesMap.TryGetValue(keyEvent.Keycode, out mode);
    }
}