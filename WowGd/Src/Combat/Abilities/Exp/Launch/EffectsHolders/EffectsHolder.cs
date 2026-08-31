using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Effects;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Gatherers;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Combat.Abilities.Exp.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

public partial class EffectsHolder : Node, IEffectsHolder
{
    private readonly ITrigger    _trigger  = null!;
    private readonly IGatherer   _gatherer = null!;
    private readonly IEffect[]   _effects  = null!;

    public EffectsHolder() {}
    public EffectsHolder(TriggerData trigger, GathererData gatherer, Array<Effect> effects)
    {
        _trigger    = trigger.Build();
        _gatherer   = gatherer.Build();
        _effects    = [.. effects];

        _trigger.Triggered += OnTriggered;
    }

    private void OnTriggered(ActuatePayload payload, TargetIntent intent)
    {
        TargetsPayload targets = _gatherer.Gather(payload, intent);
        foreach (IEffect effect in _effects)
            effect.Apply(targets);
    }

    public void Launch(ActuatePayload payload) =>
        _trigger.Start(payload);
}