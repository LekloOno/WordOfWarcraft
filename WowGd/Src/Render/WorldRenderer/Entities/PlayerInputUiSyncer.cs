using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WowGd.Src.Input.Hands;
using WowGd.Src.Render.Ui.Input;

namespace WowGd.Src.Render.WorldRenderer.Entities;

public static class PlayerInputUiSyncer
{
    private static readonly Dictionary<IListenableHandInputMode, IHandInputUi> _modesRegistry = [];

    public static void Sync()
    {
        IReadOnlyList<IFirstHandInputMode> firstHand = HandsInputManager.FirstHand;
        IReadOnlyList<ISecondHandInputMode> secondHand = HandsInputManager.SecondHand;

        int fIndex = 0, sIndex = 0;
        int fCount = firstHand.Count, sCount = secondHand.Count;

        while (fIndex < fCount || sIndex < sCount)
        {
            if (fIndex < fCount && firstHand[fIndex] is not ITwoHandedInputMode)
            {
                SyncPushed(firstHand[fIndex]);
                fIndex++;
                continue;
            }

            if (sIndex < sCount && secondHand[sIndex] is not ITwoHandedInputMode)
            {
                SyncPushed(secondHand[sIndex]);
                sIndex++;
                continue;
            }

            SyncPushed(firstHand[fIndex]);
            fIndex++;
            sIndex++;
        }
    }

    private static void SyncPushed(IHandInputMode mode)
    {
        if (mode.TryInto(out IHandInputUi? ui))
            ui.PushToContext();
    }

    public static void Register(IHandInputUi handInputUi) =>
        _modesRegistry.Add(handInputUi.InputMode, handInputUi);

    private static bool TryInto(this IHandInputMode mode, [NotNullWhen(true)] out IHandInputUi? handInputUi)
    {
        handInputUi = null;
        if (mode is not IListenableHandInputMode listenable)
            return false;

        return _modesRegistry.TryGetValue(listenable, out handInputUi);
    }
}