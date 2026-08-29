using System;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers.Exceptions;

/// <summary>
/// Thrown when the target query could not complete due to an external query it relied on being cancelled.
/// <para>
/// For example, in a player driven query : <br/>
/// 
/// We await for the player to confirm a selection through UI, but he either cancels it, or the gameplay made it impossible to continue (player death, stun, etc.)
/// </para>
/// </summary>
[Serializable]
public class TargetQueryCancelledException : Exception
{
    public TargetQueryCancelledException() : base() { }
    public TargetQueryCancelledException(string message) : base(message) { }
    public TargetQueryCancelledException(string message, Exception inner) : base(message, inner) { }
}