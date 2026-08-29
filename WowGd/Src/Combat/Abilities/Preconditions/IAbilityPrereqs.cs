using WowGd.Src.Combat.Abilities.Targets.Payload;

namespace WowGd.Src.Combat.Abilities.Preconditions;

/// <summary>
/// Describes validation steps to be completed before entering the Actuation stage.
/// 
/// For example, a cost in a specific resource like mana, a cool down, etc.
/// It could simply be gates, like a cool down, or a spell you can only use passed a given threshold, or actual costs, that mutate the entity, like reducing mana.
/// 
/// The prerequisite might modify the `size`, for example, a spell that can be casted with less mana, but for smaller effects.
/// </summary>
public interface IAbilityPrecondition
{
    bool Check(TargetsPayload targetsPayload, float size, out float resultSize);
    /// <summary>
    /// Checks 
    /// </summary>
    /// <param name="targetsPayload"></param>
    /// <param name="size"></param>
    /// <param name="resultSize"></param>
    /// <returns></returns>
    bool PeekCheck(TargetsPayload targetsPayload, float size);
}