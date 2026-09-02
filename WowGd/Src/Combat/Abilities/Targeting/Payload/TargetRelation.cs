using System;

namespace WowGd.Src.Combat.Abilities.Targeting.Payload;

/// <summary>
/// Describes the relation a given target has to
/// the entity that initiated the targeting query.
/// </summary>
[Flags]
public enum TargetRelation
{
    None    = 0,
    Self    = 1 << 0,
    Ally    = 1 << 1,
    Enemy   = 1 << 2,
}