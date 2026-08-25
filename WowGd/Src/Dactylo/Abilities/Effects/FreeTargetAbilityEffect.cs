using Godot;
using WowGd.Src.Dactylo.Abilities.Targets;
using WowGd.Src.Entities;

namespace WowGd.Src.Dactylo.Abilities.Effects;

[GlobalClass]
public abstract partial class FreeTargetAbilityEffect : Resource, IAbilityEffect<FreeTarget>
{
    public abstract bool Apply(IEntity launcher, FreeTarget target, float size);
}