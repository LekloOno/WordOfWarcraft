using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Physics;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities;

[GlobalClass]
public partial class Entity : Node, IEntity
{
    [Export]
    public EntityIdData IdData {get; private set;} = null!;

    [Export]
    public TeamMask TeamMask {get; private set;}

    public IBody Body {get; private set;} = null!;
    public IEntityHealth Health { get; private set; } = EnvironmentHealth.Instance;
    public IResourceManager ResourceManager { get; private set; } = null!;

    public IActuatorDriver ActuatorDriver { get; private set; } = null!;
    public ITargetIntentDriver TargetIntentDriver { get; private set; } = null!;

    public IWishDir WishDir { get; private set; } = null!;

    private EntityMover _entityMover;
    public IEntityMover EntityMover => _entityMover;

    private readonly TaskCompletionSource _initialized =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Initialization => _initialized.Task;

    public Entity() { _entityMover = new(this); }

    public override void _Ready()
    {
        FetchBaseComponents();

        foreach (Node child in GetChildren())
            InitComponent(child);

        _entityMover.Bind(Health);

        Health.Resurrect();

        EntitiesRegistry.Register(this);
        _initialized.TrySetResult();
    }

    public override void _EnterTree()
    {
        if (_initialized.Task.IsCompleted)
            EntitiesRegistry.Register(this);
    }

    public override void _ExitTree()
    {
        EntitiesRegistry.Unregister(this);
    }

    private void FetchBaseComponents()
    {
        if (this.TryGetComponent(out IEntityHealth? health))
            Health = health;

        if (this.TryGetComponent(out IResourceManager? resourceManager))
            ResourceManager = resourceManager;

        if (this.TryGetComponent(out IBody? body))
        {
            Body = body;
            Body.CollisionLayer = TeamMask.ToCollisionLayer();
            Body.CollisionMask  = TeamMask.ToCollisionMask();
        }

        if (this.TryGetComponent(out IActuatorDriver? actuatorDriver))
            ActuatorDriver = actuatorDriver;

        if (this.TryGetComponent(out ITargetIntentDriver? targetIntentDriver))
            TargetIntentDriver = targetIntentDriver;
    
        if (this.TryGetComponent(out IWishDir? wishDir))
            WishDir = wishDir;
    }

    private void InitComponent(Node child)
    {
        if (child is IEntityHealthBinder bindable)
            bindable.Bind(Health);
        if (child is IEntityHealthHandler healthHandler)
            healthHandler.Bind(Health);
    }

    public override void _PhysicsProcess(double delta)
    {
        _entityMover.RunChannels((float) delta);
    }
}