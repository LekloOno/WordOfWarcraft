namespace WowGd.Src.Dactylo.Worders;

/// <summary>
/// A helper to (un)bind worder handling components to their worder component.
/// </summary>
public static class WorderBinder
{
    public static void Bind(this IWorderHandler handler, IWorder worder)
    {
        (handler as IWorderCharHandler).Bind(worder);
        (handler as IWorderEraseHandler).Bind(worder);
        (handler as IWorderEraseAllHandler).Bind(worder);
        (handler as IWorderStreamHandler).Bind(worder);
    }

    public static void Unbind(this IWorderHandler handler, IWorder worder)
    {
        (handler as IWorderCharHandler).Unbind(worder);
        (handler as IWorderEraseHandler).Unbind(worder);
        (handler as IWorderEraseAllHandler).Unbind(worder);
        (handler as IWorderStreamHandler).Unbind(worder);
    }

    public static void Bind(this IWorderCharHandler handler, IWorder worder)
    {
        worder.CharHit      += handler.OnCharHit;
        worder.CharMissed   += handler.OnCharMissed;
        worder.CharMixed    += handler.OnCharMixed;
    }

    public static void Unbind(this IWorderCharHandler handler, IWorder worder)
    {
        worder.CharHit      -= handler.OnCharHit;
        worder.CharMissed   -= handler.OnCharMissed;
        worder.CharMixed    -= handler.OnCharMixed;
    }

    public static void Bind(this IWorderEraseHandler handler, IWorder worder)
    {
        worder.EraseHit      += handler.OnEraseHit;
        worder.EraseMissed   += handler.OnEraseMissed;
        worder.EraseMixed    += handler.OnEraseMixed;
    }

    public static void Unbind(this IWorderEraseHandler handler, IWorder worder)
    {
        worder.EraseHit      -= handler.OnEraseHit;
        worder.EraseMissed   -= handler.OnEraseMissed;
        worder.EraseMixed    -= handler.OnEraseMixed;
    }

    public static void Bind(this IWorderEraseAllHandler handler, IWorder worder)
    {
        worder.EraseAllHit      += handler.OnEraseAllHit;
        worder.EraseAllMissed   += handler.OnEraseAllMissed;
        worder.EraseAllMixed    += handler.OnEraseAllMixed;
    }

    public static void Unbind(this IWorderEraseAllHandler handler, IWorder worder)
    {
        worder.EraseAllHit      -= handler.OnEraseAllHit;
        worder.EraseAllMissed   -= handler.OnEraseAllMissed;
        worder.EraseAllMixed    -= handler.OnEraseAllMixed;
    }

    public static void Bind(this IWorderStreamHandler handler, IWorder worder)
    {
        worder.Completed    += handler.OnCompleted;
        worder.WordStarted  += handler.OnWordStarted;
    }

    public static void Unbind(this IWorderStreamHandler handler, IWorder worder)
    {
        worder.Completed    -= handler.OnCompleted;
        worder.WordStarted  -= handler.OnWordStarted;
    }
}