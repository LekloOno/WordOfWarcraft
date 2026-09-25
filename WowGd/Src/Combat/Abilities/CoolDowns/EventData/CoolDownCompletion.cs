using System.Diagnostics.CodeAnalysis;

namespace WowGd.Src.Combat.Abilities.CoolDowns.EventData;

public readonly struct CoolDownCompletion
{
    public readonly CoolDownCompletionMode Mode;
    /// <summary>
    /// When the cooldown ends on a cd reduction, the reduction might be greater than the remaining time.
    /// This holds such overflow.
    /// </summary>
    private readonly ulong _overflow;

    public CoolDownCompletion(ulong overflow)
    {
        Mode = CoolDownCompletionMode.Reduced;
        _overflow = overflow;
    }

    public CoolDownCompletion(bool cancelled)
    {
        Mode = cancelled
            ? CoolDownCompletionMode.Cancelled
            : CoolDownCompletionMode.Passive;

        _overflow = 0;
    }

    public bool TryGetOverflow([NotNullWhen(true)] out ulong? overflow)
    {
        overflow = _overflow;
        return Mode == CoolDownCompletionMode.Reduced;
    }
}