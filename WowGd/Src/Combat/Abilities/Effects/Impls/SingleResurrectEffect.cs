using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Effects.Impls;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
public partial class SingleResurrectEffect : SingleAbilityEffect
{
    /// <summary>
    /// 0 or less means full _hp
    /// </summary>
    [Export] private float _hp = 10f;

    public override void Effect(IEntity entity, bool IsDirect) =>
        Resurrect(entity, _size);

    public override void LauncherEffect(IEntity entity) =>
        Resurrect(entity, _size);

    private void Resurrect(IEntity entity, float size)
    {
        int? hp = _hp > 0 ? Mathf.FloorToInt(_hp * size) : null;
        entity.Health.Resurrect(hp);
    }
}