using Godot;

public static class LoacalizedKeyExt
{
    public static string LocalizedKeyName(this InputEventKey key)
    {
        return key.PhysicalKeycode switch
        {
            Key.Space => "KEY_SPACE",
            Key.Enter => "KEY_ENTER",
            Key.Escape => "KEY_ESCAPE",
            Key.Tab => "KEY_TAB",
            Key.Shift => "KEY_SHIFT",
            Key.Ctrl => "KEY_CTRL",
            Key.Alt => "KEY_ALT",

            _ => char.ConvertFromUtf32((int)key.Unicode)
        };
    }
}