using System.Threading;
using CombatCore.Primitives;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Resources;

public static class ResourceRegistry
{
    private static int _count = int.MinValue;

    private static ResourceId Register() =>
        new(Interlocked.Increment(ref _count));

    public static ResourceId GetId<T>() =>
        IdHolder<T>.Value;

    private static class IdHolder<T>
    {
        public static readonly ResourceId Value = Register();
    }

    // Resource base is private to avoid creating unexpected IdHolder.
    // resource types are explicit.
    private static T? Resource<T>(this IEntity entity) where T: class, IStandardResource
    {
        if (!entity.ResourceManager.TryGetResource(GetId<IEntityHealth>(), out IStandardResource? std))
            return null;

        if (std is not T resource)
            return null;

        return resource;
    }

    public static IEntityHealth? HealthRes(this IEntity entity) =>
        entity.Resource<IEntityHealth>();

    public static IFocus? FocusRes(this IEntity entity) =>
        entity.Resource<IFocus>();
}