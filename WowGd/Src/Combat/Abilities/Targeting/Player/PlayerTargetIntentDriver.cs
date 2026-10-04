using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Targeting.Player;

[GlobalClass]
public partial class PlayerTargetIntentDriver : Node, ITargetIntentDriver
{
    private IEntity _entity = null!;
    public readonly PlayerDirectTargetDriver DirectDriver = new();

    public override void _Ready()
    {
        if (this.TryGetComposed(out IEntity? entity))
            _entity = entity;

        AddChild(DirectDriver);
    }

    public Task<TargetResult> RetrieveTargetIntent(IEntity caster, TargetIntentAcquirer method, CancellationToken ct, IEnumerable<ITargetRule>? rules = null)
    {
        return method switch
        {
            TargetIntentAcquirer.Self => TargetIntentDriverExt.GetSelfTargetResult(caster),

            TargetIntentAcquirer.Melee => TargetIntentDriverExt.GetTackleTargetResult(caster),
            
            TargetIntentAcquirer.Direct => DirectDriver.RetrieveTarget(caster, rules, ct),
            
            TargetIntentAcquirer.Free =>
                throw new NotImplementedException(),

            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };
    }
}