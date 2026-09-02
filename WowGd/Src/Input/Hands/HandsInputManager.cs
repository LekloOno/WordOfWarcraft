using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace WowGd.Src.Input.Hands;


/// <summary>
/// Handles stack of input modes, separated by hand.
/// 
/// The idea is that any input mode associated to hand 1 can be active while an input mode of hand 2 is active.
/// 
/// But two input modes of the same hand can't be active at the same time.
/// 
/// Movement, for example, is tied to input of the hand 1.
/// Selecting abilities, to the hand 2.
/// Dacylography, to both hands.
/// </summary>
public static class HandsInputManager
{
    // List instead of stack for easier and clearer to read preemptive checks 
    private static readonly List<IFirstHandInputMode>  _firstHand  = [];
    private static readonly List<ISecondHandInputMode> _secondHand = [];

    public static bool TryPushFirstHandMode(IFirstHandInputMode mode) =>
        TryPushHandMode(mode, _firstHand);
    
    public static bool TryPushSecondHandMode(ISecondHandInputMode mode) =>
        TryPushHandMode(mode, _secondHand);

    public static bool TryPopFirstHandMode() =>
        TryPopHandMode(target: _firstHand, sibling: _secondHand);

    public static bool TryPopSecondHandMode() =>
        TryPopHandMode(target: _secondHand, sibling: _firstHand);

    public static bool TryPushTwoHandedMode(ITwoHandedInputMode mode)
    {
        if (!mode.CanStart())
            return false;

        if (TryPeek(_firstHand, out IFirstHandInputMode? fMode)
            && !fMode.CanStop())
            return false;

        if (TryPeek(_secondHand, out ISecondHandInputMode? sMode)
            && !sMode.CanStop())
            return false;
        

        fMode?.Stop();
        sMode?.Stop();

        _firstHand.Add(mode);
        _secondHand.Add(mode);

        mode.Start();
        return true;
    }

    public static bool TryPopTwoHandedMode()
    {
        if (!TryPeek(_firstHand, out IFirstHandInputMode? fThMode))
            return !TryPeek(_secondHand, out _);

        if (!TryPeek(_secondHand, out ISecondHandInputMode? sThMode))
            return false;
        
        if (sThMode != fThMode || fThMode is not ITwoHandedInputMode)
            return false;
        
        if (!fThMode.CanStop() || !sThMode.CanStop())
            return false;

        if (TryPeek(_firstHand, out IFirstHandInputMode? fMode, 2)
            && !fMode.CanStart())
            return false;

        if (TryPeek(_secondHand, out ISecondHandInputMode? sMode, 2)
            && !sMode.CanStart())
            return false;
        
        Pop(_firstHand);
        Pop(_secondHand);

        fThMode.Stop();
        sThMode.Stop();
        
        fMode?.Start();
        sMode?.Start();

        return true;
    }

    private static bool TryPushHandMode<T>(T mode, List<T> hand)
        where T : IHandInputMode
    {
        if (!mode.CanStart())
            return false;

        // Nothing special about two handed modes in enable.
        // If the top mode is a two handed one, it'll get disabled anyways, no special check to do.
        if (TryPeek(hand, out T? current)
            && !current.CanStop())
            return false;

        current?.Stop();
        hand.Add(mode);
        mode.Start();

        return true;
    }

    private static bool TryPopHandMode<T, U>(List<T> target, List<U> sibling)
        where T : IHandInputMode
        where U : IHandInputMode
    {
        if (!TryPeek(target, out T? current))   // Nothing to disable, result is expected.
            return true;

        if (!current.CanStop())                 // There's something to stop
            return false;                       // But current state doesn't allow it to.

        if (!TryPeek(target, out T? next, 2))   // Nothing to enable, and the thing to stop can be.
        {
            Pop(target);
            current.Stop();
            return true;
        }

        if (next is ITwoHandedInputMode twoHanded)  // If the next is two handed, we need to check
        {                                           // whether its sibling mode is itself.
            bool plainTwoHanded = sibling[^1] is ITwoHandedInputMode secondTwoHanded
                && secondTwoHanded == twoHanded;

            if (plainTwoHanded && !twoHanded.CanStart())
                return false;

            Pop(target);
            current.Stop();

            if (plainTwoHanded)
                twoHanded.Start();

            return true;
        }

        if (!next.CanStart())
            return false;

        Pop(target);
        current.Stop();
        next.Start();
        return true;
    }

    private static bool TryPeek<T>(List<T> hand, [NotNullWhen(true)] out T? mode, int depth = 1)
        where T : IHandInputMode
    {
        if (hand.Count < depth)
        {
            mode = default;
            return false;
        }

        mode = hand[^depth];
        return true;
    }

    private static void Pop<T>(List<T> hand)
        where T : IHandInputMode
    {
        hand.RemoveAt(hand.Count - 1);
    }
}