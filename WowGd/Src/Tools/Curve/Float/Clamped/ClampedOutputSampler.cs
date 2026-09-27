using Godot;

namespace WowGd.Src.Tools.Curve.Float.Clamped;

[GlobalClass]
public abstract partial class ClampedOutputSampler : FloatCurveSampler
{
    [Export] private float _minOutput;
    [Export] private float _maxOutput;

    public override float Sample(float value)
    {
        float ratio = GetRatio(value);
        return _maxOutput * ratio + _minOutput * (1 - ratio);
    }

    /// <summary>
    /// Gives a ratio from _minOutput to _maxOuput, which means the return value should be between 0 and 1.
    /// </summary>
    /// <param name="value">the input value</param>
    /// <returns>The output value ratio, between 0 and 1</returns>
    protected abstract float GetRatio(float value);
}