using System;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

[Flags]
public enum AccelerationWeighting
{
    Acceleration    = 1 << 0,
    Duration        = 1 << 1,
}