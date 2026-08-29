using System;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers.Exceptions;

/// <summary>
/// Thrown when a response couldn't be produced in time.
/// </summary>
[Serializable]
public class TargetQueryTimedOutException : Exception
{
    public TargetQueryTimedOutException() : base() { }
    public TargetQueryTimedOutException(string message) : base(message) { }
    public TargetQueryTimedOutException(string message, Exception inner) : base(message, inner) { }
}