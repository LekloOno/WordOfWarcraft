using System;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Stats.Modifier;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;

[GlobalClass]
public partial class SingleStatsEffect : SingleAbilityEffect
{
    public override string Id => "effect_single_stats";
    [Export] private BatchModifier _modifiers = null!;
    [Export] private float _duration;

    public override void Effect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        if (!StaticTree.TryGetTree(out SceneTree? tree))
            return;

        if (_modifiers.ApplyTo(target, gatherWeight * actuateWeight) is not IDisposable handle)
            return;
        
        tree.CreateTimer(_duration).Timeout += handle.Dispose;
    }
}