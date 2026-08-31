using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Data.Resources;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Triggers;

/// <summary>
/// Base abstraction of a trigger data.
/// 
/// It might seem pointless, it is.
/// The only point of having such base abstraction is godot-editability.
/// This allows us to have an easy "export-compatible" type for any kind of trigger data, since interfaces are not exportable.
/// </summary>
[GlobalClass]
public abstract partial class TriggerData : Resource, ITriggerData, IComponentFactory<ITrigger>
{
    public abstract string Id { get; }
    public abstract ITrigger Build();
}