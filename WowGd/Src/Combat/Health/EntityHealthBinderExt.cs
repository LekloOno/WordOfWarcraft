using Godot;
using WowGd.Src.Combat.Resources;

namespace WowGd.Src.Combat.Health;

/// <summary>
/// A helper to (un)bind health handling components to their health component.
/// </summary>
public static class EntityHealthBinderExt
{
    public static void Bind(this IEntityHealthHandler handler, IEntityHealth health)
    {
        health.Died         += handler.OnDied;
        health.Resurrected  += handler.OnResurrected;

        (handler as IStandardResourceHandler).Bind(health);
    }

    public static void Unbind(this IEntityHealthHandler handler, IEntityHealth health)
    {
        health.Died         -= handler.OnDied;
        health.Resurrected  -= handler.OnResurrected;

        (handler as IStandardResourceHandler).Unbind(health);
    }

    public static void BindChildren(this Node self, IEntityHealth health)
    {
        foreach (Node node in self.GetChildren())
            if (node is IEntityHealthHandler handler)
                handler.Bind(health);
    }

    public static void UnbindChildren(this Node self, IEntityHealth health)
    {
        foreach (Node node in self.GetChildren())
            if (node is IEntityHealthHandler handler)
                handler.Unbind(health);
    }
}