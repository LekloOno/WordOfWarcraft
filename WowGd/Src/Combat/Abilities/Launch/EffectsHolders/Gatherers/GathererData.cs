using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;

/// <summary>
/// Base abstraction of an gatherer data.
/// 
/// It might seem pointless, it is.
/// The only point of having such base abstraction is godot-editability.
/// This allows us to have an easy "export-compatible" type for any kind of gatherer data, since interfaces are not exportable.
/// </summary>
[GlobalClass]
public abstract partial class GathererData : Resource, IGathererData, IComponentFactory<IGatherer>
{
    public abstract string Id { get; }
    public abstract IGatherer Build();
}