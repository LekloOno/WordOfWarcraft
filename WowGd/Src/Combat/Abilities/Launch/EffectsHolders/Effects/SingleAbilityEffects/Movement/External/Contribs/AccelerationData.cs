using Godot;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

[GlobalClass]
public partial class AccelerationData : Resource, IMoveContribData
{
    [Export] private float _acceleration = 2f;
    [Export] private float _duration = 1f;
    [Export] private AccelerationWeighting _weighting =
        AccelerationWeighting.Acceleration | AccelerationWeighting.Duration;

    public bool Start(IEntityMover mover, Vector2 direction, float weight, bool strict = false)
    {
        float acceleration = _acceleration;
        if (_weighting.HasFlag(AccelerationWeighting.Acceleration))
            acceleration *= weight;

        float duration = _duration;
        if (_weighting.HasFlag(AccelerationWeighting.Duration))
            duration *= weight;

        return new AccelerationContrib(mover, direction, acceleration, duration, strict).Start();
    }
}