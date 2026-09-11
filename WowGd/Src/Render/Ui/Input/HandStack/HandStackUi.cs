using System.Collections.Generic;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Render.Ui.Input.HandStack;

public abstract class HandStackUi : IHandStackUi
{
    protected readonly List<IHandInputUi> _firstHand = [];
    protected readonly List<IHandInputUi> _secondHand = [];

    public void PushUi(IHandInputUi handInputUi)
    {
        if (handInputUi.InputMode is ITwoHandedInputMode)
            PushTwoHandedUi(handInputUi);
        else if (handInputUi.InputMode is IFirstHandInputMode)
            PushFirstHandUi(handInputUi);
        else if (handInputUi.InputMode is ISecondHandInputMode)
            PushSecondHandUi(handInputUi);
    }

    public void RemoveUi(IHandInputUi handInputUi)
    {
        if (handInputUi.InputMode is ITwoHandedInputMode)
            RemoveTwoHandedUi(handInputUi);
        else if (handInputUi.InputMode is IFirstHandInputMode)
            RemoveFirstHandUi(handInputUi);
        else if (handInputUi.InputMode is ISecondHandInputMode)
            RemoveSecondHandUi(handInputUi);
    }

    protected abstract void PushTwoHandedUi(IHandInputUi handInputUi);
    protected abstract void PushFirstHandUi(IHandInputUi handInputUi);
    protected abstract void PushSecondHandUi(IHandInputUi handInputUi);

    protected abstract void RemoveSecondHandUi(IHandInputUi handInputUi);
    protected abstract void RemoveFirstHandUi(IHandInputUi handInputUi);
    protected abstract void RemoveTwoHandedUi(IHandInputUi handInputUi);
}