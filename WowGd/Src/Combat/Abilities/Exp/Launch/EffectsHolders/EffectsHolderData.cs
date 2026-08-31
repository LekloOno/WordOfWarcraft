using System.Collections.Generic;
using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Data.Resources;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Effects;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Gatherers;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

public partial class EffectsHolderData : Resource, IEffectsHolderData, IComponentFactory<IEffectsHolder>
{
    [Export] public string Id { get; private set; } = string.Empty;

    [Export] private TriggerData    _trigger  = null!;
    [Export] private GathererData   _gatherer = null!;
    [Export] private Array<Effect>  _effects  = [];

    public ITriggerData     Trigger  => _trigger;
    public IGathererData    Gatherer => _gatherer;
    public List<IEffect>    Effects  => [.. _effects];

    public IEffectsHolder Build() =>
        new EffectsHolder(_trigger, _gatherer, _effects);
}