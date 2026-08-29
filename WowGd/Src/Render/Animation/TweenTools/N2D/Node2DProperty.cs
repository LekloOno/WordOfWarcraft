namespace WowGd.Src.Render.Animation.TweenTools.N2D;

public enum Node2DProperty
{
    Position,
    GlobalPosition,
    Scale,
    Rotation,
}


public static class Node2DPropertyExt
{
    public static string ToPath(this Node2DProperty prop) => prop switch
    {
        Node2DProperty.Position => "position",
        Node2DProperty.GlobalPosition => "global_position",
        Node2DProperty.Scale => "scale",
        Node2DProperty.Rotation => "rotation",
        _ => throw new System.ArgumentOutOfRangeException(nameof(prop))
    };
}
