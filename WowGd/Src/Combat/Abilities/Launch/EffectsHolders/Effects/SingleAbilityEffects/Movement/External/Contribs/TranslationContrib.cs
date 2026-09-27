using Godot;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Physics.Movement.Channels;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

public class TranslationContrib(IEntityMover mover, Vector2 direction, float distance, float duration, bool strict = false) : IContributor
{
    private float _elapsed = 0f;

    public bool Start() =>
        mover.AddContributor(MovementChannels.External, this, 0, strict).HasFlag(ChannelSubResult.Success);

    public Contribution GetContribution(EntityMover mover, float delta)
    {
        _elapsed += delta;

        float effDuration = Mathf.Max(delta, duration);
        float strength = distance / effDuration;

        Contribution contrib = new(rawVelocity: direction * strength);

        if (_elapsed >= duration)
            mover.QueueRemoveContributor(MovementChannels.External, this);

        return contrib;
    }

    public void OnChannelClosed() { }
    public void OnChannelOpened() { }
}