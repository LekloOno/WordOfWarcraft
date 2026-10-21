using System.Threading.Tasks;
using CombatCore;
using CombatCore.Primitives;
using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Stats;
using WowGd.Src.Physics;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Combat.Abilities.Mage;

public partial class Breach : IEntity
{
    public EntityIdData IdData { get; }
    public TeamMask TeamMask { get; }
    public IBody Body => this;
    public IResourceManager ResourceManager => ResourceManagerSink.Sink;
    public IWishDir WishDir => WishDirSink.Sink;
    public IActuatorDriver ActuatorDriver => null!;
    public ITargetIntentDriver TargetIntentDriver => null!;
    public IEntityMover EntityMover => MoverSink.Sink;
    public IStatistics Statistics => StatisticSink.Sink;
    public Task Initialization => Task.CompletedTask;
    public EntityId Id { get; private set; }
    public IResourcePort Resources => ResourceManagerSink.Sink;
    public IStatPort Stats => StatisticSink.Sink;

    public void SetId(EntityId id) => Id = id;
}