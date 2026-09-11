namespace WowGd.Src.Combat.Resources.FocusRes;

public static class StandardResourceBinderExt
{
    public static void Bind(this IStandardResourceHandler handler, IStandardResource focus)
    {
        focus.Consumed  += handler.OnConsumed;
        focus.Generated += handler.OnGenerated;
    }

    public static void Unbind(this IStandardResourceHandler handler, IStandardResource focus)
    {
        focus.Consumed  -= handler.OnConsumed;
        focus.Generated -= handler.OnGenerated;
    }
}