using System.Collections.Generic;
using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Data.Resources;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Effects;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Gatherers;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

[GlobalClass]
public partial class EffectsHolderData : Resource, IEffectsHolderData, IComponentFactory<IEffectsHolder>
{
    [Export] public string Id { get; private set; } = string.Empty;

    [Export] public TriggerData     TriggerDt   { get; private set; } = null!;
    [Export] public GathererData    GathererDt  { get; private set; } = null!;
    [Export] public Array<Effect>   EffectsDt   { get; private set; } = [];

    public ITriggerData     Trigger  => TriggerDt;
    public IGathererData    Gatherer => GathererDt;
    public List<IEffect>    Effects  => [.. EffectsDt];

    public IEffectsHolder Build() =>
        new EffectsHolder(this);
}