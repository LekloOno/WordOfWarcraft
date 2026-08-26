using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets;

/// <summary>
/// Describes the relation a given target has to
/// the entity that initiated the targeting query.
/// </summary>
public enum TargetRelation
{
    Self,
    Ally,
    Enemy,
}

/// <summary>
/// Represents a target, resulting of a targeting query.
/// </summary>
/// <param name="entity">The targeted entity.</param>
/// <param name="relation">
/// The relation this target has to the query launcher (see TargetsPayload.Launcher).
/// This typically abstracts away `TeamMask` relations as a much more straight forward, "side agnostic", and functionnal data.
/// </param>
/// <param name="isDirect">
/// Whether the target results from an indirect area or explicit targeting.
/// </param>
public readonly struct Target(IEntity entity, TargetRelation relation, bool isDirect)
{
    public readonly IEntity         Entity      = entity;
    public readonly TargetRelation  Relation    = relation;
    public readonly bool            IsDirect    = isDirect;
}