using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Entities.Health;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities;

[GlobalClass]
public partial class Entity : Node
{
    private IEntityHealth _health = EnvironmentHealth.Instance;
    public override void _Ready()
    {
        FetchBaseComponents();

        foreach (Node child in GetChildren())
            InitComponent(child);
    }

    private void FetchBaseComponents()
    {
        if (this.TryGetComponent(out IEntityHealth? health))
        {
            _health = health;
            _health.Resurrect();
        }
    }

    private void InitComponent(Node child)
    {
        if (child is IEntityHealthHandler healthHandler)
            healthHandler.Bind(_health);
    }
}