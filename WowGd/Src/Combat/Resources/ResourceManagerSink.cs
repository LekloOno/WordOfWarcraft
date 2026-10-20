using System.Diagnostics.CodeAnalysis;
using CombatCore;
using CombatCore.Primitives;

namespace WowGd.Src.Combat.Resources;

public class ResourceManagerSink : IResourceManager
{
    public readonly static ResourceManagerSink Sink = new();
    private readonly static ResourceChange _sinkChange = new(0, 0, false);
    public ResourceChange Apply(ResourceId id, ResourceDirection direction, long amount) => _sinkChange;
    public bool Has(ResourceId id) => false;
    public bool TryGetResource(ResourceId id, [NotNullWhen(true)] out IStandardResource? resource)
    {
        resource = null;
        return false;
    }
}