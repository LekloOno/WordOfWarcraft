using Godot;

namespace WowGd.Src.Dactylo.Abilities.Targets;

public readonly struct FreeTarget(Vector2 position, uint teamMask) : ITarget
{
    public readonly Vector2 Position = position;
    public readonly uint TeamMask = teamMask;
}