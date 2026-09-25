using System.Collections.Generic;
using Godot;

namespace WowGd.Src.Combat.Abilities.Data;

/// <summary>
/// A stateless representation of an ability, pure data.
/// </summary>
public interface IAbilityData
{
    string Id { get; }
    Texture2D Icon { get; }
    /// <summary>
    /// It could be a simple rule, but as it might be very common, it's simple to store and expose it independantly.
    /// 0 means no cooldown.
    /// </summary>
    ulong Base { get; }

    IReadOnlyList<ICasterRule>  StartPreconditions      { get; }
    /// <summary>
    /// The rules that apply to acquire targets, relative to the states of the caster and given target intent.
    /// <br/>
    /// An intent is either a position, or another entity.
    /// <br/>
    /// Order does not matter, preconditions should not mutate anything.
    /// </summary>
    IReadOnlyList<ITargetRule>  TargetRules             { get; }
    /// <summary>
    /// The mean of acquiring targets.
    /// 
    /// Melee, direct, free or self targeting.
    /// It does not conduct the logic of targeting itself, but what component of the entity we're calling to retrieve such target.
    /// 
    /// This makes the ability description compatible with both an AI or a player.
    /// A player would redirect to an input based interraction, while the AI will retrieve it through its decision algorithm.
    /// </summary>
    TargetIntentAcquirer        TargetIntentAcquirer    { get; }
}