using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Effects.Impls;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public partial class SingleHealEffect : SingleAbilityEffect
{
    [Export] private float _hp = 10f;

    public override void Effect(IEntity entity, bool IsDirect) =>
        Heal(entity, _size);

    public override void LauncherEffect(IEntity entity) =>
        Heal(entity, _size);

    private void Heal(IEntity entity, float size) =>
        entity.Health.Heal(Mathf.FloorToInt(_hp * size));
}