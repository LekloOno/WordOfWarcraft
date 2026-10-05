namespace WowGd.Src.Combat.Resources;

public static class StandardResourceBinderExt
{
    public static void Bind(this IStandardResourceHandler handler, IStandardResource resource)
    {
        resource.Consumed   += handler.OnConsumed;
        resource.Generated  += handler.OnGenerated;
        resource.MaxChanged += handler.OnMaxChanged;
    }

    public static void Unbind(this IStandardResourceHandler handler, IStandardResource resource)
    {
        resource.Consumed   -= handler.OnConsumed;
        resource.Generated  -= handler.OnGenerated;
        resource.MaxChanged -= handler.OnMaxChanged;
    }
}