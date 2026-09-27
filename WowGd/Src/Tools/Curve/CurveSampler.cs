using Godot;

namespace WowGd.Src.Tools.Curve;

public abstract partial class CurveSampler<T> : Resource
{
    public abstract T Sample(T value);
}