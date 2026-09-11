namespace WowGd.Src.Render.Ui.Input.HandStack;

public class ActiveHandStackUi : HandStackUiDispatcher
{
    protected override void DisableAction(IHandInputUi handInputUi) =>
        handInputUi.SetUnactive();

    protected override void EnableAction(IHandInputUi handInputUi) =>
        handInputUi.SetActive();
}