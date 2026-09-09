namespace WowGd.Src.Render.Ui.Input;

public enum HandInputDocking
{
    /// <summary>
    /// None means it can always be displayed, using its own process -
    /// Should not be overidden by upcoming ui nor override previous ui.
    /// </summary>
    None,
    /// <summary>
    /// Main is the main central split dock.
    /// </summary>
    Main,
    /// <summary>
    /// Screen means it is a screen space ui, that must override/be overriden by other screen docked uis.
    /// </summary>
    Screen,
}
