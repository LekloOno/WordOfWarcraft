using WowGd.Src.Combat.Abilities.Exp.Actuation.Drivers;
using WowGd.Src.Combat.Health;
using WowGd.Src.Physics;

namespace WowGd.Src.Entities;

public interface IEntity
{
    /// <summary>
    /// Data that identifies the entity.
    /// </summary>
    EntityIdData    IdData          { get; }
    /// <summary>
    /// Allows to regroup entities into teams.
    /// </summary>
    uint            TeamMask        { get; }
    /// <summary>
    /// The physical body of the entity, notably defines its position.
    /// </summary>
    IBody           Body            { get; }
    /// <summary>
    /// The health of the entity.
    /// </summary>
    IEntityHealth   Health          { get; }
    IActuatorDriver ActuatorDriver  { get; }
}