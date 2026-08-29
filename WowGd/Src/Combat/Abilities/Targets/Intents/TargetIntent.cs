using Godot;
using System.Collections.Generic;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targets.Intents;

public readonly struct TargetIntent(ICollection<IEntity> direct, ICollection<Vector2> indirect)
{
    public readonly ICollection<IEntity> Direct = direct;
    public readonly ICollection<Vector2> Indirect = indirect;
}