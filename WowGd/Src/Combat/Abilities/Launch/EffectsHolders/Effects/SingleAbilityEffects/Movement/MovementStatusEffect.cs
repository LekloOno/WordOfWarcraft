using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Physics.Movement.Status;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement;

[GlobalClass]
public partial class MovementStatusEffect : SingleAbilityEffect
{
    public override string Id => "movement_status_effect";
    [Export] private MovementStatus _movementStatus;
    [Export] private float _duration;
    [Export] private bool _gatherWeighted = false;
    [Export] private bool _actuateWeighted = false;

    public override void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        entity.EntityMover.StatusChannels.Query(this, _movementStatus, out _);
        
        if (!StaticTree.TryGetTree(out SceneTree? tree))
            return;

        float duration = _duration
            * (_gatherWeighted ? gatherWeight : 1f)
            * (_actuateWeighted ? actuateWeight : 1f);

        tree.CreateTimer(duration).Timeout +=
            () => entity.EntityMover.StatusChannels.Unquery(this, _movementStatus, out _);
    }

    public override void LauncherEffect(IEntity entity, float actuateWeight)
    {
        entity.EntityMover.StatusChannels.Query(this, _movementStatus, out _);
    }
}