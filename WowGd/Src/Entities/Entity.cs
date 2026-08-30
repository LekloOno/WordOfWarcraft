using Godot;
using WowGd.Src.Combat.Abilities.Exp.Actuation.Drivers;
using WowGd.Src.Combat.Health;
using WowGd.Src.Physics;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities;

[GlobalClass]
public partial class Entity : Node, IEntity
{
    [Export]
    public EntityIdData IdData {get; private set;} = null!;

    [Export(PropertyHint.Layers2DPhysics)]
    public uint TeamMask {get; private set;}

    public IBody Body {get; private set;} = null!;
    public IEntityHealth Health { get; private set; } = EnvironmentHealth.Instance;

    public IActuatorDriver ActuatorDriver { get; private set; } = null!;

    public override void _Ready()
    {
        FetchBaseComponents();

        foreach (Node child in GetChildren())
            InitComponent(child);

        Health.Resurrect();

        _initialized = true;
        EntitiesRegistry.Register(this);
    }

    private bool _initialized = false;
    public override void _EnterTree()
    {
        if (_initialized)
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

        if (this.TryGetComponent(out IBody? body))
            Body = body;

        if (this.TryGetComponent(out IActuatorDriver? driver))
            ActuatorDriver = driver;
    }

    private void InitComponent(Node child)
    {
        if (child is IEntityHealthBinder bindable)
            bindable.Bind(Health);
        if (child is IEntityHealthHandler healthHandler)
            healthHandler.Bind(Health);
    }
}