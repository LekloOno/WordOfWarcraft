namespace WowGd.Src.Combat.Resources.FocusRes;

public static class FocusBinderExt
{
    public static void Bind(this IFocusHandler handler, IFocus focus)
    {
        focus.Consumed  += handler.OnConsumed;
        focus.Generated += handler.OnGenerated;
    }

    public static void Unbind(this IFocusHandler handler, IFocus focus)
    {
        focus.Consumed  -= handler.OnConsumed;
        focus.Generated -= handler.OnGenerated;
    }
}