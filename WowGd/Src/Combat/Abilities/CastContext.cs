using WowGd.Src.Combat.Abilities.Effects;
using WowGd.Src.Combat.Abilities.Targets.Intents;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities;

public sealed class CastContext(IEntity launcher)
{
    public readonly IEntity Launcher = launcher;
    public TargetIntent Intent;
    public TargetIntent EffectiveIntent;
    public TargetsPayload Payload;
    public readonly EffectDebt Debt = new();
    public readonly CastModifiers Modifiers = new();
}