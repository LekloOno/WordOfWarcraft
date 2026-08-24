using Godot;

namespace WowGd.Src.Render.Animation.TweenTools.Value;

[GlobalClass]
public partial class FloatTweenValue : TweenValue
{
    [Export] private float _value;
    public override Variant Value => _value;
}