using System;
using System.Collections.Generic;
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

    public bool Query(object querier, MovementStatus query)
    {
        if (_requester.ContainsKey(querier))
            return false;

        _requester.Add(querier, query);
        IncrementRequesters(query);

        return true;
    }

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