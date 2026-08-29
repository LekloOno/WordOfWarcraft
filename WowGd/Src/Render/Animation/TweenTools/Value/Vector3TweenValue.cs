using Godot;

namespace WowGd.Src.Render.Animation.TweenTools.Value;

[GlobalClass]
public partial class Vector3TweenValue : TweenValue
{
    [Export] private Vector3 _value;
    public override Variant Value => _value;
}
