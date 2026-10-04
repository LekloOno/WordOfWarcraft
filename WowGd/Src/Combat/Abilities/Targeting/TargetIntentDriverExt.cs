using System;
using System.Threading.Tasks;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting;

public static class TargetIntentDriverExt
{
    public static bool TryGetTackleIntent(IEntity entity, out TargetIntent targetIntent)
    {
        if (!entity.EntityMover.DynamicTackleNode.TryGetTackled(out IEntity? tackled))
        {
            targetIntent = default;
            return false;
        }

        targetIntent = new TargetIntent(tackled);
        return true;
    }

    public static Task<TargetResult> GetTackleTargetResult(IEntity caster)
    {
        if (!caster.EntityMover.DynamicTackleNode.TryGetTackled(out IEntity? tackled))
            return TryGetTackleable(caster);
        return Task.FromResult(TargetResult.Ok(new TargetIntent(tackled)));
    }

    private static Task<TargetResult> TryGetTackleable(IEntity caster)
    {
        if (caster.TryGetClosestTarget(out IEntity? target, caster.EnemyMask(), true) &&
            caster.EntityMover.DynamicTackleNode.StartTackle(target, 1f))
            return Task.FromResult(TargetResult.Ok(new TargetIntent(target)));

        return Task.FromResult(TargetResult.Fail(TargetFailure.NoTarget));
    }

    public static Task<TargetResult> GetSelfTargetResult(IEntity caster) =>
        Task.FromResult(TargetResult.Ok(new TargetIntent(caster)));
}