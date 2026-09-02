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
    private readonly PlayerDirectTargetDriver _directDriver = new();

    public override void _Ready()
    {
        AddChild(_directDriver);
    }

    public Task<TargetIntent> RetrieveTargetIntent(IEntity caster, TargetIntentAcquirer method, CancellationToken ct, IEnumerable<ITargetRule>? rules = null)
    {
        return method switch
        {
            TargetIntentAcquirer.Self =>
                throw new InvalidOperationException(
                "Unexpected self target intent request, should be handled by the state machine itself."),
            
            TargetIntentAcquirer.Melee =>
                throw new NotImplementedException(),
            
            TargetIntentAcquirer.Direct =>
                _directDriver.RetrieveTarget(caster, rules, ct),
            
            TargetIntentAcquirer.Free =>
                throw new NotImplementedException(),

            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };
    }
}