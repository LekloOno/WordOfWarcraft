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

    public override void Effect(IEntity entity, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        entity.EntityMover.StatusChannels.Query(this, _movementStatus, out _);
        
        if (!StaticTree.TryGetTree(out SceneTree? tree))
            return;

        float duration = _duration * gatherWeight * actuateWeight;

        tree.CreateTimer(duration).Timeout +=
            () => entity.EntityMover.StatusChannels.Unquery(this, _movementStatus, out _);
    }

    public override void LauncherEffect(IEntity entity, float actuateWeight) =>
        Effect(entity, true, 1f, actuateWeight);
}