using Godot;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Physics.Movement.Channels;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

public class ImpulseContrib(IEntityMover mover, Vector2 direction, float strength, bool strict = false) : IContributor
{
    public bool Start() =>
        mover.AddContributor(MovementChannels.External, this, 0, strict).HasFlag(ChannelSubResult.Success);

    public Contribution GetContribution(EntityMover mover, float delta)
    {
        mover.QueueRemoveContributor(MovementChannels.External, this);
        return new(acceleration: direction * strength);
    }

    public void OnChannelClosed() { }
    public void OnChannelOpened() { }
}