using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CombatCore;
using CombatCore.Primitives;
using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Resources;

[GlobalClass]
public partial class ResourceManager : Node, IResourceManager
{
    private readonly Dictionary<int, IStandardResource> _resources = [];

    public ResourceChange Apply(ResourceId id, ResourceDirection direction, long amount)
    {
        if (!_resources.TryGetValue(id.Value, out IStandardResource? resource))
            return new();
        
        long before = resource.Current;

        switch (direction)
        {
            case ResourceDirection.Add :
                resource.Generate((int)amount, out _);
                break;
            case ResourceDirection.Remove :
                resource.Consume((int)amount, out _);
                break;
        }
        
        long after = resource.Current;
        bool dead = resource is IEntityHealth health && health.Dead();

        return new(before, after, dead);
    }

    public bool Has(ResourceId id) =>
        _resources.ContainsKey(id.Value);

    public bool TryGetResource(ResourceId id, [NotNullWhen(true)] out IStandardResource? resource) =>
        _resources.TryGetValue(id.Value, out resource);

    public override void _Ready()
    {
        if (this.TryGetComponent(out IFocus? focus, false))
            _resources.TryAdd(ResourceRegistry.GetId<IFocus>().Value, focus);
        if (this.TryGetComponent(out IEntityHealth? health))
            _resources.TryAdd(ResourceRegistry.GetId<IEntityHealth>().Value, health);
    }
}