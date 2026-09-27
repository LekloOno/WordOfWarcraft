using System;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects;

/// <summary>
/// Defines which weight should be taken into account for an effect.
/// </summary>
[Flags]
public enum EffectWeighting
{
    None    = 0,
    Gather  = 1 << 0,
    Actuate = 1 << 1,
}