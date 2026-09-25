namespace WowGd.Src.Combat.Abilities.CoolDowns.EventData;

public readonly struct CoolDownModification(ulong @base, ulong eff, ulong prev, ulong mod)
{
    private readonly CoolDownEventData _baseData = new(@base, eff);
    /// <summary>
    /// Base standard cooldown.
    /// </summary>
    public ulong Base       => _baseData.Base;
    /// <summary>
    /// Final effective remaining cooldown.
    /// </summary>
    public ulong Effective  => _baseData.Effective;
    /// <summary>
    /// Remaining cooldown before the modification.
    /// </summary>
    public readonly ulong Previous = prev;
    /// <summary>
    /// Effective modification.
    /// </summary>
    public readonly ulong Modification = mod;
}