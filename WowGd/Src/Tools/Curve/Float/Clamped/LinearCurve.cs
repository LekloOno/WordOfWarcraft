using Godot;

namespace WowGd.Src.Tools.Curve.Float.Clamped;

[GlobalClass]
public partial class LinearCurve : ClampedOutputSampler
{
    /// <summary>
    /// Maps to output value _minOutput
    /// </summary>
    [Export] private float _minInput;
    /// <summary>
    /// Maps to output value _maxOutput
    /// </summary>
    [Export] private float _maxInput;

    protected override float GetRatio(float value) =>
        Mathf.Clamp((value - _minInput) / (_maxInput - _minInput), 0f, 1f);
}