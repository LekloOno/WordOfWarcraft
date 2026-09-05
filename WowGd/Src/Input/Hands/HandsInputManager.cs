using System;
using System.Collections.Generic;
using System.Text;
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
    private static readonly List<IFirstHandInputMode> _firstHand = [];
    private static readonly List<ISecondHandInputMode> _secondHand = [];

    public static bool TryPushFirstHandMode(IFirstHandInputMode mode)
    {
        if (mode is ITwoHandedInputMode)
            return false;

        return TryTransition(mode, TopOrNull(_secondHand), () => _firstHand.Add(mode));
    }

    public static bool TryPushSecondHandMode(ISecondHandInputMode mode)
    {
        if (mode is ITwoHandedInputMode)
            return false;

        return TryTransition(TopOrNull(_firstHand), mode, () => _secondHand.Add(mode));
    }

    public static bool TryPushTwoHandedMode(ITwoHandedInputMode mode) =>
        TryTransition(mode, mode, () =>
        {
            _firstHand.Add(mode);
            _secondHand.Add(mode);
        });

    public static bool TryPopFirstHandMode() =>
        _firstHand.Count == 0 || TryRemoveAndReconcile(_firstHand.Count - 1, null);

    public static bool TryPopSecondHandMode() =>
        _secondHand.Count == 0 || TryRemoveAndReconcile(null, _secondHand.Count - 1);

    public static bool TryPopTwoHandedMode()
    {
        if (_firstHand.Count == 0 && _secondHand.Count == 0)
            return true;

        var (activeFirst, activeSecond) = ComputeActive(TopOrNull(_firstHand), TopOrNull(_secondHand));
        if (activeFirst is not ITwoHandedInputMode || !ReferenceEquals(activeFirst, activeSecond))
            return false; // nothing genuinely (fully) active to pop

        return TryRemoveAndReconcile(_firstHand.Count - 1, _secondHand.Count - 1);
    }

    public static bool TryRemoveFirstHandMode(IFirstHandInputMode mode)
    {
        if (mode is ITwoHandedInputMode)
            return false;

        int index = _firstHand.LastIndexOf(mode);
        return index < 0 || TryRemoveAndReconcile(index, null);
    }

    public static bool TryRemoveSecondHandMode(ISecondHandInputMode mode)
    {
        if (mode is ITwoHandedInputMode)
            return false;

        int index = _secondHand.LastIndexOf(mode);
        return index < 0 || TryRemoveAndReconcile(null, index);
    }

    public static bool TryRemoveTwoHandedMode(ITwoHandedInputMode mode)
    {
        int fIndex = _firstHand.LastIndexOf(mode);
        int sIndex = _secondHand.LastIndexOf(mode);
        if (fIndex < 0 && sIndex < 0)
            return true;

        if (fIndex < 0 || sIndex < 0)
            throw new InvalidOperationException("Orphan two-handed mode: present in one hand's stack only.");

        return TryRemoveAndReconcile(fIndex, sIndex);
    }

    private static T? TopOrNull<T>(List<T> hand, int depth = 1) where T : class =>
        hand.Count > depth - 1 ? hand[^depth] : null;

    /// <summary>
    /// A two-handed mode is only genuinely active when it's on top of BOTH stacks
    /// at once. On top of only one, that hand has nothing active - it's waiting
    /// for the other hand to converge back onto it.
    /// </summary>
    private static (IFirstHandInputMode? First, ISecondHandInputMode? Second) ComputeActive(
        IFirstHandInputMode? topF, ISecondHandInputMode? topS)
    {
        bool converged = topF is ITwoHandedInputMode f && ReferenceEquals(f, topS);
        return (
            topF is ITwoHandedInputMode && !converged ? null : topF,
            topS is ITwoHandedInputMode && !converged ? null : topS
        );
    }

    /// <summary>
    /// Given the tops the stacks WOULD have after a mutation, works out which modes
    /// should be active, checks the transition is legal, and if so, applies it.
    /// </summary>
    private static bool TryTransition(
        IFirstHandInputMode? newFirstTop, ISecondHandInputMode? newSecondTop, Action onCommit)
    {
        var (beforeFirst, beforeSecond) = ComputeActive(TopOrNull(_firstHand), TopOrNull(_secondHand));
        var (afterFirst, afterSecond) = ComputeActive(newFirstTop, newSecondTop);

        // A much broader definition of what should be started/stopped
        // We store, with ref identity, the top modes before and after.
        // It also prevents from double start/stop
        var before = new HashSet<IHandInputMode>(ReferenceEqualityComparer.Instance);
        if (beforeFirst is not null) before.Add(beforeFirst);
        if (beforeSecond is not null) before.Add(beforeSecond);

        var after = new HashSet<IHandInputMode>(ReferenceEqualityComparer.Instance);
        if (afterFirst is not null) after.Add(afterFirst);
        if (afterSecond is not null) after.Add(afterSecond);

        var toStop = new List<IHandInputMode>();
        foreach (var m in before) if (!after.Contains(m)) toStop.Add(m);

        var toStart = new List<IHandInputMode>();
        foreach (var m in after) if (!before.Contains(m)) toStart.Add(m);

        foreach (var m in toStop) if (!m.CanStop())
            return false;

        foreach (var m in toStart) if (!m.CanStart())
            return false;

        foreach (var m in toStop) m.Stop();
        onCommit();
        foreach (var m in toStart) m.Start();
        return true;
    }

    /// <summary>
    /// Removes the given index from each stack (a null index is left untouched),
    /// after checking what the removal reveals.
    /// If an index isn't the top of its list, removing it can't change what's
    /// active there.
    /// 
    /// If it IS the top, the entry underneath (if any) becomes candidate to start.
    /// </summary>
    private static bool TryRemoveAndReconcile(int? firstIndex, int? secondIndex)
    {
        IFirstHandInputMode? newFirstTop =
            firstIndex == _firstHand.Count - 1
                ? TopOrNull(_firstHand, 2)
                : TopOrNull(_firstHand);

        ISecondHandInputMode? newSecondTop =
            secondIndex == _secondHand.Count - 1
                ? TopOrNull(_secondHand, 2)
                : TopOrNull(_secondHand);

        return TryTransition(newFirstTop, newSecondTop, () =>
        {
            if (firstIndex is int fi) _firstHand.RemoveAt(fi);
            if (secondIndex is int si) _secondHand.RemoveAt(si);
        });
    }


    private static void DebugStack()
    {
        StringBuilder f = new();

        foreach (IFirstHandInputMode fh in _firstHand)
            f.Append($"{fh.GetType().Name},");

        StringBuilder s = new();
        foreach (ISecondHandInputMode sh in _secondHand)
            s.Append($"{sh.GetType().Name},");

        GD.Print($"\nf : {f}\ns : {s}");
    }
}