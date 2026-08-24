using Godot;

namespace WowGd.Src.Render.Animation.TweenTools.Value;

[GlobalClass]
public partial class ColorTweenValue : TweenValue
{
    [Export] private Color _value;
    public override Variant Value => _value;
}