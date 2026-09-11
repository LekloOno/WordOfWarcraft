namespace WowGd.Src.Render.Ui.Input.HandStack;

public class DockHandStackUi : HandStackUiDispatcher
{
    protected override void EnableAction(IHandInputUi handInputUi) =>
        handInputUi.ShowHand();

    protected override void DisableAction(IHandInputUi handInputUi) =>
        handInputUi.HideHand();
}
