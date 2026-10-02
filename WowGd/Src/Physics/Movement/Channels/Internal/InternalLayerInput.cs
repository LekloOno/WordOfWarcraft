using Godot;

namespace WowGd.Src.Physics.Movement.Channels.Internal;

public readonly ref struct InternalLayerInput(EntityMover mover, float maxSpeed, Vector2 wishDir)
{
    public readonly EntityMover Mover = mover;
    public readonly float MaxSpeed = maxSpeed;
    public readonly Vector2 WishDir = wishDir;
}