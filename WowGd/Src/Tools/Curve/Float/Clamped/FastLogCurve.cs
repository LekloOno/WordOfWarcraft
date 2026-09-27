using Godot;

namespace WowGd.Src.Tools.Curve.Float.Clamped;

/// <summary>
/// https://www.desmos.com/calculator/oqfxjchzwx?lang=fr
/// </summary>
[GlobalClass]
public partial class FastLogCurve : ClampedOutputSampler
{
    /// <summary>
    /// The input value at which half max ouput is reached.
    /// </summary>
    [Export] private float _halfInput;

    protected override float GetRatio(float value)
    {
        float divisor = value + _halfInput;
        if (divisor == 0f)
            return 1f;

        return Mathf.Clamp(value / divisor, 0f, 1f);
    }
}