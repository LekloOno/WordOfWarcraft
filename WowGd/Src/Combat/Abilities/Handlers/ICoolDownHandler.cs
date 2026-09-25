using WowGd.Src.Combat.Abilities.CoolDowns.EventData;

namespace WowGd.Src.Combat.Abilities.Handlers;

public interface ICoolDownHandler
{
    void OnCdStartedAt(CoolDownEventData cdData);
    void OnCdReduced(CoolDownModification cdReduction);
    void OnCdEnlengthed(CoolDownModification cdElongation);
    void OnCdCompleted(CoolDownCompletion completion);
}