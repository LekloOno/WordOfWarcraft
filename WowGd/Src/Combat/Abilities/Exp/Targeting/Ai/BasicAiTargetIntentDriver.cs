using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting.Ai;

[GlobalClass]
public partial class BasicAiTargetIntentDriver : Node, ITargetIntentDriver
{
    public Task<TargetIntent> RetrieveTargetIntent(IEntity caster, TargetIntentAcquirer method, CancellationToken ct, IEnumerable<ITargetRule>? rules = null)
    {
        if(caster.TryGetClosestTarget(out IEntity? target, ~caster.TeamMask))
            return Task.FromResult(new TargetIntent(target));

        GD.PushError($"No entity to be found by {nameof(BasicAiTargetIntentDriver)}. Silently returning Vector2.Zero target.");
        return Task.FromResult(new TargetIntent(Vector2.Zero));
    }
}