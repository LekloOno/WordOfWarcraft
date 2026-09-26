using Godot;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects;

[GlobalClass]
public partial class DashData : Resource
{
    [Export] public float Distance { get; private set; }
    [Export] public float Duration { get; private set; }

    public float Strenght(float delta)
    {
        float duration = Mathf.Max(delta, Duration);
        return Distance / duration;
    }
}