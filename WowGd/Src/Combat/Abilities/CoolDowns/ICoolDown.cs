using System;
using WowGd.Src.Combat.Abilities.CoolDowns.EventData;
using WowGd.Src.Combat.Abilities.Data;

namespace WowGd.Src.Combat.Abilities.CoolDowns;

public interface ICoolDown
{
    public ICoolDownData CoolDownData { get; }
    public ulong Remaining { get; }

    /// <summary>
    /// Whether the cooldown is currently unactive.
    /// </summary>
    /// <returns></returns>
    public bool Completed();
    /// <summary>
    /// Start the cooldown.
    /// </summary>
    public void StartCd();
    /// <summary>
    /// Cancel on going cooldown.
    /// </summary>
    public void CancelCd();
    /// <summary>
    /// Forces the cooldown at the given time in ms.
    /// </summary>
    public void StartCdAt(ulong timeMs);
    /// <summary>
    /// Shorten the on going cooldown of the given time in ms.
    /// </summary>
    /// <param name="timeMs"></param>
    public void ReduceCd(ulong timeMs);
    /// <summary>
    /// Enlength the on going cooldown of the given time in ms.
    /// </summary>
    /// <param name="timeMs"></param>
    public void EnlengthCd(ulong timeMs);

    event Action<CoolDownEventData>? CdStartedAt;
    event Action<CoolDownModification>? CdReduced;
    event Action<CoolDownModification>? CdEnlenghted;
    event Action<CoolDownCompletion>? CdCompleted;
}