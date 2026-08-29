using Godot;

namespace WowGd.Src.Combat.Health;

/// <summary>
/// A helper to (un)bind health handling components to their health component.
/// </summary>
public static class EntityHealthBinderExt
{
    public static void Bind(this IEntityHealthHandler handler, IEntityHealth health)
    {
        health.Died         += handler.OnDied;
        health.Damaged      += handler.OnDamaged;
        health.Healed       += handler.OnHealed;
        health.Resurrected  += handler.OnResurrected;
    }

    public static void Unbind(this IEntityHealthHandler handler, IEntityHealth health)
    {
        health.Died         -= handler.OnDied;
        health.Damaged      -= handler.OnDamaged;
        health.Healed       -= handler.OnHealed;
        health.Resurrected  -= handler.OnResurrected;
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