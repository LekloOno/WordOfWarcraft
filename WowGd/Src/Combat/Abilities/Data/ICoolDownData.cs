namespace WowGd.Src.Combat.Abilities.Data;

public interface ICoolDownData
{
    /// <summary>
    /// It could be a simple rule, but as it might be very common, it's simple to store and expose it independantly.
    /// 0 means no cooldown.
    /// </summary>
    ulong Base { get; }
}