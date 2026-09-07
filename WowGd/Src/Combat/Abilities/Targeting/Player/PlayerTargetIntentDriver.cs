using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Targeting.Player;

[GlobalClass]
public partial class PlayerTargetIntentDriver : Node, ITargetIntentDriver
{
    public readonly PlayerDirectTargetDriver DirectDriver = new();

    public override void _Ready()
    {
        AddChild(DirectDriver);
    }

    public Task<TargetIntent> RetrieveTargetIntent(IEntity caster, TargetIntentAcquirer method, CancellationToken ct, IEnumerable<ITargetRule>? rules = null)
    {
        return method switch
        {
            TargetIntentAcquirer.Self => Task.FromResult(new TargetIntent(caster)),
            
            TargetIntentAcquirer.Melee =>
                throw new NotImplementedException(),
            
            TargetIntentAcquirer.Direct =>
                DirectDriver.RetrieveTarget(caster, rules, ct),
            
            TargetIntentAcquirer.Free =>
                throw new NotImplementedException(),

            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };
    }
}