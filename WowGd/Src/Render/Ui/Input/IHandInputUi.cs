using WowGd.Src.Input.Hands;

namespace WowGd.Src.Render.Ui.Input;

public interface IHandInputUi
{
    /// <summary>
    /// The user shared input ui context.
    /// It could theorically be a singleton manager of all client's hand input uis.
    /// This will handle the final display of combined hand input uis, accross different docks.
    /// </summary>
    IHandsInputUiContext Context { get; }
    /// <summary>
    /// The kind of docking to use for that Ui.
    /// </summary>
    HandInputDocking Docking { get; }
    /// <summary>
    /// The input mode this ui component is tied to.
    /// </summary>
    IListenableHandInputMode InputMode { get; }
    /// <summary>
    /// Unlike Input mode themselves, hand input ui dis/enabling should always succeed.
    /// It has no reason to fail.
    /// </summary>
    void ShowHand();
    /// <summary>
    /// Unlike Input mode themselves, hand input ui dis/enabling should always succeed.
    /// It has no reason to fail.
    /// </summary>
    void HideHand();
    void SetActive();
    void SetUnactive();
}
