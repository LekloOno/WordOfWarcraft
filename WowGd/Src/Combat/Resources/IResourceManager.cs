using System.Diagnostics.CodeAnalysis;
using CombatCore;
using CombatCore.Primitives;

namespace WowGd.Src.Combat.Resources;

public interface IResourceManager : IResourcePort
{
    bool TryGetResource(ResourceId id, [NotNullWhen(true)] out IStandardResource? resource);
}