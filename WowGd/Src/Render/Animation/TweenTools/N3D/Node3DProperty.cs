namespace WowGd.Src.Render.Animation.TweenTools.N3D;

public enum Node3DProperty
{
    Position,
    GlobalPosition,
    Scale,
    Rotation,
}


public static class Node3DPropertyExt
{
    public static string ToPath(this Node3DProperty prop) => prop switch
    {
        Node3DProperty.Position => "position",
        Node3DProperty.GlobalPosition => "global_position",
        Node3DProperty.Scale => "scale",
        Node3DProperty.Rotation => "rotation",
        _ => throw new System.ArgumentOutOfRangeException(nameof(prop))
    };
}
