using Godot;

namespace WowGd.Src.Render.Animation.TweenTools.N3D;

[GlobalClass]
public partial class Node3DTweenSettings : TweenSettings
{
    [Export] public Node3DProperty Property { get; set; }

    public override PropertyTweener? TweenProperty(Tween tween, GodotObject target, Variant? value = null, string? propertyPath = null)
        => base.TweenProperty(tween, target, value, Property.ToPath());
}
