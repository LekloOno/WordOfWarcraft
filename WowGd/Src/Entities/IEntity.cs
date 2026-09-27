using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Physics;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities;

public interface IEntity : IInitializable
{
    /// <summary>
    /// Data that identifies the entity.
    /// </summary>
    EntityIdData        IdData              { get; }
    /// <summary>
    /// Allows to regroup entities into teams.
    /// </summary>
    TeamMask            TeamMask            { get; }
    /// <summary>
    /// The physical body of the entity, notably defines its position.
    /// </summary>
    IBody               Body                { get; }
    /// <summary>
    /// The health of the entity.
    /// </summary>
    IEntityHealth       Health              { get; }
    IResourceManager    ResourceManager     { get; }
    IWishDir            WishDir             { get; }

    IActuatorDriver     ActuatorDriver      { get; }
    ITargetIntentDriver TargetIntentDriver  { get; }

    IEntityMover        EntityMover         { get; }
}