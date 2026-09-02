using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting;

public interface ITargetIntentDriver
{
    Task<TargetIntent> RetrieveTargetIntent(
        IEntity caster,
        TargetIntentAcquirer method, CancellationToken ct,
        IEnumerable<ITargetRule>? rules = null
    );
}