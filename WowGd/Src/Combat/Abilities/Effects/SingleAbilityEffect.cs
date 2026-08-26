using Godot;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Effects;

/// <summary>
/// Single in that it performs the same effect for every target matchin the specified TargetRelation.
/// </summary>
[GlobalClass]
public abstract partial class SingleAbilityEffect : AbilityEffect
{
    [Export] private bool _launcher;
    [Export] private TargetRelation _target;

    protected float _size {get; private set;}

    public override bool Apply(TargetsPayload targetsPayload, float size)
    {
        _size = size;

        if (_launcher)
            LauncherEffect(targetsPayload.Launcher);

        if (_target == TargetRelation.None)
            return true;

        targetsPayload.Apply(
            _target.HasSelf() ? Effect : null,
            _target.HasAlly() ? Effect : null,
            _target.HasEnemy() ? Effect : null
        );

        return true;
    }

    public abstract void Effect(IEntity entity, bool IsDirect);
    public abstract void LauncherEffect(IEntity entity);
}