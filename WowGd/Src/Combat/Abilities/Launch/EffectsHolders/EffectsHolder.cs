using CombatCore.Sources;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Triggers;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders;

public class EffectsHolder : IEffectsHolder
{
    private readonly EffectsHolderData _data;
    private readonly ITrigger    _trigger;
    private readonly IGatherer   _gatherer;

    public EffectsHolder(EffectsHolderData data)
    {
        _data = data;
        
        _trigger    = data.TriggerDt.Build();
        _gatherer   = data.GathererDt.Build();

        _trigger.Triggered += OnTriggered;
    }

    private void OnTriggered(ActuatePayload payload, TargetIntent intent, Source source, Source? cause)
    {
        TargetsPayload targets = _gatherer.Gather(payload, intent);
        foreach (IEffect effect in _data.EffectsDt)
            effect.Apply(targets, source, cause);
    }

    public void Launch(ActuatePayload payload, Source source, Source? cause) =>
        _trigger.Start(payload, source, cause);
}