using WowGd.Src.Entities;

namespace WowGd.Src.Dactylo.Abilities.Targets;

public readonly struct DirectTarget : ITarget
{
    public readonly IEntity Entity;
}