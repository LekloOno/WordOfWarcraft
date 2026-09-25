namespace WowGd.Src.Combat.Abilities.CoolDowns.EventData;

public readonly struct CoolDownEventData(ulong @base, ulong eff)
{
    public readonly ulong Base = @base;
    public readonly ulong Effective = eff;
}