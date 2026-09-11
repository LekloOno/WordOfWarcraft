namespace WowGd.Src.Render.Ui.Input.HandStack;

public interface IHandStackUi
{
    void PushUi(IHandInputUi handInputUi);
    void RemoveUi(IHandInputUi handInputUi);
}