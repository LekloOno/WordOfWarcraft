using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Targets.Payload;

namespace WowGd.Src.Combat.Abilities.Effects;

[GlobalClass]
public partial class ComposedAbilityEffect : AbilityEffect
{
    [Export] private Array<SingleAbilityEffect> _effects = [];

    public override bool Apply(TargetsPayload targetsPayload, float size)
    {
        foreach (SingleAbilityEffect effect in _effects)
            effect.Apply(targetsPayload, size);

        return true;
    }
}