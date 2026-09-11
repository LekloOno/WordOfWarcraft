using Godot;
using WowGd.Src.Input.Generators;

namespace WowGd.Src.Render.Ui.Input.Impls.MoveMode;

public enum WasdEnum
{
    Up,
    Down,
    Left,
    Right,
}

public static class WasdEnumExt
{
    public static string ToActionName(this WasdEnum wasd) => wasd switch
    {
        WasdEnum.Up => StandardKeyGenerator.Up,
        WasdEnum.Down => StandardKeyGenerator.Down,
        WasdEnum.Left => StandardKeyGenerator.Left,
        WasdEnum.Right => StandardKeyGenerator.Right,
        _ => throw new System.NotImplementedException(),
    };
    
    public static bool IsPressedEcho(this WasdEnum wasd, InputEventKey key) =>
        key.IsActionPressed(wasd.ToActionName(), true);

    public static bool IsJustPressed(this WasdEnum wasd, InputEventKey key) =>
        key.IsActionPressed(wasd.ToActionName());

    public static bool IsJustReleased(this WasdEnum wasd, InputEventKey key) =>
        key.IsActionReleased(wasd.ToActionName());
}