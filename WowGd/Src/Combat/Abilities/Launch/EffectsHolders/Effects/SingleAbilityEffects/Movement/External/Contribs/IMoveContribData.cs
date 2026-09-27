using Godot;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

public interface IMoveContribData
{
    bool Start(IEntityMover mover, Vector2 direction, float weight, bool strict = false);
}