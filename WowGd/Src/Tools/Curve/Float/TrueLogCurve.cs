using System;
using Godot;

namespace WowGd.Src.Tools.Curve.Float.Clamped;

/// <summary>
/// https://www.desmos.com/calculator/bwasnhs9lc
/// </summary>
[GlobalClass]
public partial class TrueLogCurve : FloatCurveSampler
{
    /// <summary>
    /// The output for an input value of 0.
    /// </summary>
    [Export] private float _nullOutput;
    /// <summary>
    /// A target input value for which `_referenceOutput` is reached. <br/>
    /// <c>Sample(_referenceInput) = referenceOutput</c>.
    /// </summary>
    [Export] private float _referenceInput;
    /// <summary>
    /// The output value for an input value of `_referenceInput`. <br/>
    /// <c>Sample(_referenceInput) = referenceOutput</c>.
    /// </summary>
    [Export] private float _referenceOutput;

    public override float Sample(float value)
    {
        float ratio = MathF.Log(1f + (value / _referenceInput), 2f);
        return Mathf.Lerp(_nullOutput, _referenceOutput, ratio);
    }
}