using CombatCore;
using Godot;
using Godot.Collections;
using WowGd.Src.Entities.Stats.Modifier;

namespace WowGd.Src.Combat.Statuses;

[GlobalClass]
public partial class StatusEffects : Resource
{
    [Export] private Array<SingleStatModifier> _modifiers = [];

    public StatEffect[] Build()
    {
        StatEffect[] effects = new StatEffect[_modifiers.Count];
        
        for (int i = 0; i < _modifiers.Count; i ++)
            effects[i] = new(_modifiers[i].Contribution);

        return effects;
    }
}