using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Gatherers;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Combat.Abilities.Exp.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

public partial class EffectsHolder : IEffectsHolder
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

    private void OnTriggered(ActuatePayload payload, TargetIntent intent)
    {
        TargetsPayload targets = _gatherer.Gather(payload, intent);
        foreach (IEffect effect in _data.EffectsDt)
            effect.Apply(targets);
    }

    public void Launch(ActuatePayload payload) =>
        _trigger.Start(payload);
}