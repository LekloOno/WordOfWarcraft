using System.Threading;
using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Exp.Data;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting;

public interface ITargetIntentDriver
{
    Task<TargetIntent> RetrieveTargetIntent(TargetIntentAcquirer method, CancellationToken ct);
}