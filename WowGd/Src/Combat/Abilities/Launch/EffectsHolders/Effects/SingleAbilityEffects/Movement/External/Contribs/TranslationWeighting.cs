using System;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

[Flags]
public enum TranslationWeighting
{
    Distance = 1 << 0,
    Duration = 1 << 1,
}