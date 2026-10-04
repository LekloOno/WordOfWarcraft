using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Targeting.Ai;

[GlobalClass]
public partial class BasicAiTargetIntentDriver : Node, ITargetIntentDriver
{
    private IEntity _entity = null!;

    public override void _Ready()
    {
        if (this.TryGetComposed(out IEntity? entity))
            _entity = entity;
    }

    public Task<TargetResult> RetrieveTargetIntent(IEntity caster, TargetIntentAcquirer method, CancellationToken ct, IEnumerable<ITargetRule>? rules = null)
    {
        return method switch
        {
            TargetIntentAcquirer.Self => TargetIntentDriverExt.GetSelfTargetResult(caster),

            TargetIntentAcquirer.Melee => TargetIntentDriverExt.GetTackleTargetResult(caster),

            TargetIntentAcquirer.Direct =>
                caster.TryGetClosestTarget(out IEntity? target, ~caster.TeamMask)
                    ? Task.FromResult(TargetResult.Ok(new TargetIntent(target)))
                    : Task.FromResult(TargetResult.Fail(TargetFailure.NoTarget)),

            TargetIntentAcquirer.Free => throw new System.NotImplementedException(),

            _ => throw new System.ArgumentOutOfRangeException(nameof(method)),
        };
    }
}