using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders.Effects;

/// <summary>
/// Base abstraction of an effect Data.
/// 
/// It might seem pointless, it is.
/// The only point of having such base abstraction is godot-editability.
/// This allows us to have an easy "export-compatible" type for any kind of effect, since interfaces are not exportable.
/// </summary>
[GlobalClass]
public abstract partial class Effect : Resource, IEffect
{
    public abstract string Id { get; }
    public abstract bool Apply(TargetsPayload targetsPayload);
}