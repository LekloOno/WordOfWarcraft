using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Effects.Impls;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public partial class SingleDamageEffect : SingleAbilityEffect
{
    [Export] private float _hp = 10f;

    public override void Effect(IEntity entity, bool IsDirect) =>
        Damage(entity, _size);

    public override void LauncherEffect(IEntity entity) =>
        Damage(entity, _size);

    private void Damage(IEntity entity, float size) =>
        entity.Health.Damage(Mathf.FloorToInt(_hp * size));
}