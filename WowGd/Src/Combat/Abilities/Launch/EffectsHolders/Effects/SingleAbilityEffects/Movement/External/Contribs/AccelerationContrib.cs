using Godot;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Physics.Movement.Channels;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

public class AccelerationContrib(IEntityMover mover, Vector2 direction, float acceleration, float duration, bool strict = false) : IContributor
{
    private float _elapsed = 0f;

    public bool Start() =>
        mover.AddContributor(MovementChannels.External, this, 0, strict).HasFlag(ChannelSubResult.Success);

    public Contribution GetContribution(EntityMover mover, float delta)
    {
        _elapsed += delta;

        Contribution contrib = new(acceleration: direction * acceleration * delta);

        if (_elapsed >= duration)
            mover.QueueRemoveContributor(MovementChannels.External, this);

        return contrib;
    }

    public void OnChannelClosed() { }
    public void OnChannelOpened() { }
}