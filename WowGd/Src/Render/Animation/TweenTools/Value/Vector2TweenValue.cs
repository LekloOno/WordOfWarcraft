using Godot;

namespace WowGd.Src.Render.Animation.TweenTools.Value;

[GlobalClass]
public partial class Vector2TweenValue : TweenValue
{
    [Export] private Vector2 _value;
    public override Variant Value => _value;
}