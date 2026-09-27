using Godot;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

[GlobalClass]
public partial class ImpulseData : Resource, IMoveContribData
{
    [Export] private float _strength = 15f;

    public bool Start(IEntityMover mover, Vector2 direction, float weight, bool strict = false) =>
        new ImpulseContrib(mover, direction, _strength * weight, strict).Start();
}