namespace WowGd.Src.Combat.Health;

/// <summary>
/// A helper to (un)bind health handling components to their health component.
/// </summary>
public static class EntityHealthBinder
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
}