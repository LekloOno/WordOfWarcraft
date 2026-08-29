using Godot;
using WowGd.Src.Combat.Abilities.Targets.Payload;

namespace WowGd.Src.Combat.Abilities.Effects;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public abstract partial class AbilityEffect : Resource, IAbilityEffect
{
    public abstract bool Apply(TargetsPayload targetsPayload, float size);
}