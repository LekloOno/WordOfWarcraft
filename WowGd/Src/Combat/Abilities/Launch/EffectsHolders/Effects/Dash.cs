using Godot;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Physics.Movement.Channels;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects;

public class Dash(DashData data) : IMovementContributor
{
    private Vector2 _direction;
    private readonly DashData _data = data;
    private float _elapsed;

    public bool StartTo(Vector2 direction, IEntityMover mover, bool strict = false)
    {
        _direction = direction;
        _elapsed = 0f;
        return mover.AddContributor(MovementChannels.External, this, strict);
    }

    public void GetContribution(EntityMover mover, float delta, out Vector2 force, out float frictionRatio)
    {
        _elapsed += delta;
        force = _direction * _data.Strenght(delta);

        if (_elapsed > _data.Duration)
            mover.QueueRemoveContributor(MovementChannels.External, this);

        frictionRatio = 1f;
    }

    public void OnChannelClosed() { }
    public void OnChannelOpened() { }
}