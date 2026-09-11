namespace WowGd.Src.Render.Ui.Input;

public static class HandInputUiExt
{
    public static void BindToContext(this IHandInputUi handInputUi)
    {
        handInputUi.InputMode.InputPushed   += handInputUi.PushToContext;
        handInputUi.InputMode.InputRemoved  += handInputUi.RemoveFromContext;
    }
    
    public static void UnbindToContext(this IHandInputUi handInputUi)
    {
        handInputUi.InputMode.InputPushed   -= handInputUi.PushToContext;
        handInputUi.InputMode.InputRemoved  -= handInputUi.RemoveFromContext;
    }

    public static void PushToContext(this IHandInputUi handInputUi) =>
        handInputUi.Context.TryPushUi(handInputUi, handInputUi.Docking);

    public static void RemoveFromContext(this IHandInputUi handInputUi) =>
        handInputUi.Context.TryRemoveUi(handInputUi, handInputUi.Docking);
}