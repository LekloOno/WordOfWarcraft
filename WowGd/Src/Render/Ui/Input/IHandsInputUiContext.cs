namespace WowGd.Src.Render.Ui.Input;

public interface IHandsInputUiContext
{
    /// <summary>
    /// Tries to push the given UI in the given docking mode, and returns whether the operation was successfull.
    /// If successfull, the UI should now be visible, and override any previous Ui at the given dock (if the dock is not None).
    /// </summary>
    /// <param name="handInputUi">The ui component to push.</param>
    /// <param name="docking">The docking mode for the provided ui component to be pushed in.</param>
    /// <returns>
    /// Whether the operation was successfull.
    /// Failure are implementation specs.
    /// </returns>
    bool TryPushUi(IHandInputUi handInputUi, HandInputDocking docking);
    /// <summary>
    /// Tries to remove the given UI in the given docking mode, and returns whether the operation was successfull.
    /// If successfull, and the UI was visible, it should be hidden, and if there's another component slept underneath in the docking mode,
    /// it should be awaken and visible.
    /// </summary>
    /// <param name="handInputUi">The ui component to remove.</param>
    /// <param name="docking">The docking mode for the provided ui component to be removed from.</param>
    /// <returns>
    /// Whether the operation was successfull.
    /// Removing a non-present component is a failure.
    /// Further failure can be implementation specs.
    /// </returns>
    bool TryRemoveUi(IHandInputUi handInputUi, HandInputDocking docking);
}
