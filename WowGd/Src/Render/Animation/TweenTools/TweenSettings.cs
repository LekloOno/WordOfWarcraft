using Godot;

namespace WowGd.Src.Render.Animation.TweenTools;

[GlobalClass]
public partial class TweenSettings : Resource
{
    [Export] public float AnimationTime { get; private set; }
    [Export] public Tween.TransitionType TransitionType { get; private set; }
    [Export] public Tween.EaseType EaseType { get; private set; }
    [Export] public string? PropertyPath { get; private set; }
    [Export] public TweenValue? Value { get; private set; }
    [Export] public bool Relative { get; private set; } = false;

    public virtual PropertyTweener? TweenProperty(Tween tween, GodotObject target, Variant? value = null, string? propertyPath = null)
    {
        tween.SetTrans(TransitionType);
        tween.SetEase(EaseType);

        Variant effectiveValue;
        if (value is Variant givenValue)
            effectiveValue = givenValue;
        else if (Value != null)
            effectiveValue = Value.Value;
        else
        {
            GD.PrintErr("[TweenSetting] missing Value.");
            return null;
        }

        string effectivePath;
        if (propertyPath != null)
            effectivePath = propertyPath;
        else if (PropertyPath != null)
            effectivePath = PropertyPath;
        else
        {
            GD.PrintErr("[TweenSetting] missing PropertyPath.");
            return null;
        }

        var tweener = tween.TweenProperty(target, effectivePath, effectiveValue, AnimationTime);

        if (Relative) tweener.AsRelative();

        return tweener;
    }
}
