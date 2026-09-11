using System.Collections.Generic;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Render.Ui.Input.HandStack;

public abstract class HandStackUiDispatcher : HandStackUi
{
    protected override void PushTwoHandedUi(IHandInputUi handInputUi) =>
        PushHandsUi(handInputUi, [_firstHand, _secondHand]);

    protected override void PushFirstHandUi(IHandInputUi handInputUi) =>
        PushHandsUi(handInputUi, [_firstHand]);

    protected override void PushSecondHandUi(IHandInputUi handInputUi) =>
        PushHandsUi(handInputUi, [_secondHand]);

    protected override void RemoveTwoHandedUi(IHandInputUi handInputUi) =>
        RemoveHandsUi(handInputUi, [_firstHand, _secondHand]);

    protected override void RemoveFirstHandUi(IHandInputUi handInputUi) =>
        RemoveHandsUi(handInputUi, [_firstHand]);

    protected override void RemoveSecondHandUi(IHandInputUi handInputUi) =>
        RemoveHandsUi(handInputUi, [_secondHand]);

    protected abstract void EnableAction(IHandInputUi handInputUi);
    protected abstract void DisableAction(IHandInputUi handInputUi);


    private void PushHandsUi(IHandInputUi handInputUi, List<IHandInputUi>[] handChannels)
    {
        HashSet<IHandInputUi> toDisable = [];

        foreach (List<IHandInputUi> channel in handChannels)
        {
            int lastIdx = channel.Count - 1;
            if (lastIdx >= 0)
                toDisable.Add(channel[lastIdx]);

            channel.Add(handInputUi);
        }

        foreach (IHandInputUi ui in toDisable)
            DisableAction(ui);

        EnableAction(handInputUi);
    }

    private void RemoveHandsUi(IHandInputUi handInputUi, List<IHandInputUi>[] handChannels)
    {
        IHandInputUi? toDisable = null;
        HashSet<IHandInputUi> toEnable = [];

        foreach (List<IHandInputUi> channel in handChannels)
        {
            int idx = channel.LastIndexOf(handInputUi);
            int lastIdx = channel.Count - 1;

            if (idx == lastIdx)
            {
                toDisable = handInputUi;

                if (lastIdx >= 1)
                    toEnable.Add(channel[lastIdx - 1]);
            }

            channel.Remove(handInputUi);
        }

        foreach (IHandInputUi ui in toEnable)
            TryEnableHand(ui, handChannels);

        if (toDisable != null)
            DisableAction(toDisable);
    }

    private bool TryEnableHand(IHandInputUi ui, List<IHandInputUi>[] handChannels)
    {
        if (ui.InputMode is not ITwoHandedInputMode)
        {
            EnableAction(ui);
            return true;
        }

        foreach (List<IHandInputUi> channel in handChannels)
        {
            int lastIdx = channel.Count - 1;
            if (lastIdx < 0 || channel[lastIdx] != ui)
                return false;
        }

        EnableAction(ui);
        return true;
    }
}