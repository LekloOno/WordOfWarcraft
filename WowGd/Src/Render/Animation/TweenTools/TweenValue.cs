using Godot;

namespace WowGd.Src.Render.Animation.TweenTools;

[GlobalClass]
public abstract partial class TweenValue : Resource
{
    public abstract Variant Value {get;}
}