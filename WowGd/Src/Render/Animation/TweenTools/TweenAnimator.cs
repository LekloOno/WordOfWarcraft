using Godot;
using Godot.Collections;

namespace WowGd.Src.Render.Animation.TweenTools;

[GlobalClass]
public partial class TweenAnimator : Node
{
    [Export] public Node Target = null!;
    [Export] public Array<TweenAnimation> Animations = new();

    private Tween? _currentTween;
    private readonly Dictionary<string, TweenAnimation> _lookup = new();

    public override void _Ready()
    {
        foreach (var anim in Animations)
        {
            if (!string.IsNullOrEmpty(anim.EventName))
                _lookup[anim.EventName] = anim;
        }

        // Play("move");
    }

    public void Play(string eventName)
    {
        if (!_lookup.TryGetValue(eventName, out var anim))
        {
            GD.PrintErr($"[TweenAnimator] Aucune animation pour l'évènement '{eventName}'.");
            return;
        }

        _currentTween?.Kill();
        _currentTween = CreateTween();

        if (anim.Parallel)
            _currentTween.SetParallel();

        foreach (var step in anim.Steps)
            step.TweenProperty(_currentTween, Target);
    }
}
