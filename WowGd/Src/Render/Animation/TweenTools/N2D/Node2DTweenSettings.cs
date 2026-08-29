using Godot;

namespace WowGd.Src.Render.Animation.TweenTools.N2D;

[GlobalClass]
public partial class Node2DTweenSettings : TweenSettings
{
    [Export] public Node2DProperty Property { get; set; }

    public override PropertyTweener? TweenProperty(Tween tween, GodotObject target, Variant? value = null, string? propertyPath = null)
        => base.TweenProperty(tween, target, value, Property.ToPath());
}
