using System;
using System.Collections.Generic;
using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.Status;

public class StatusQueryRegister
{
    public MovementStatus State { get; private set; } = 0;
    private readonly Dictionary<object, MovementStatus> _requester = [];
    private readonly uint[] _requesters =
        new uint[Enum.GetValues<MovementStatus>().Length];

    public event Action<MovementStatus>? Enabled;
    public event Action<MovementStatus>? Disabled;

    /// <summary>
    /// Queries for a set of status flags with this querier.
    /// <para>
    /// It never fails. In the worst case scenario, is just has no effect - if the queried flags were already all present.
    /// </para>
    /// </summary>
    /// <param name="querier">The querier.</param>
    /// <param name="query">The status flags to turn on.</param>
    /// <param name="prev">
    /// The querier's previous query flags, or the default value if no query was registered.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the querier already had a query registered;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Query(object querier, MovementStatus query, out MovementStatus prev)
    {
        bool modified = _requester.TryGetValue(querier, out prev);

        MovementStatus added = query & ~prev;
        _requester[querier] = prev | query;

        IncrementRequesters(added);

        return modified;
    }

    /// <summary>
    /// Unqueries a set of status flags from this querier.
    /// </summary>
    /// <param name="querier">The querier.</param>
    /// <param name="query">The status flags to turn off.</param>
    /// <param name="prev">
    /// The querier's previous query flags, or the default value if no query was registered.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the querier had a registered query,
    /// even if none of the specified flags were set;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Unquery(object querier, MovementStatus query, out MovementStatus prev)
    {
        if (!_requester.TryGetValue(querier, out prev))
            return false;

        MovementStatus removed = prev & query;
        MovementStatus next = prev & ~removed;

        if (next == 0)
            _requester.Remove(querier);
        else
            _requester[querier] = next;

        DecrementRequesters(removed);

        return true;
    }

    /// <summary>
    /// Unqueries all status flags from this querier.
    /// </summary>
    /// <param name="querier">The querier.</param>
    /// <returns>
    /// <see langword="true"/> if the querier had a registered query;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Unquery(object querier)
    {
        if (!_requester.TryGetValue(querier, out MovementStatus query))
            return false;

        _requester.Remove(querier);
        DecrementRequesters(query);

        return true;
    }

    private void IncrementRequesters(MovementStatus flags)
    {
        MovementStatus enabled = 0;

        foreach (int index in BitFlags.Enumerate((uint)flags))
            if (++_requesters[index] == 1)
                enabled |= (MovementStatus)(1u << index);

        State |= enabled;
        Enabled?.Invoke(enabled);
    }

    private void DecrementRequesters(MovementStatus flags)
    {
        uint bits = (uint)flags;

        MovementStatus disabled = 0;

        foreach (int index in BitFlags.Enumerate((uint)flags))
            if (--_requesters[index] == 0)
                disabled |= (MovementStatus)(1u << index);

        State &= ~disabled;
        Disabled?.Invoke(disabled);
    }
}