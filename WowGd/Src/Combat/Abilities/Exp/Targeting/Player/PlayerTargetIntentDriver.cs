using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting.Player;

[GlobalClass]
public partial class PlayerTargetIntentDriver : Node, ITargetIntentDriver
{
    private readonly PlayerDirectTargetDriver _directDriver = new();

    public override void _Ready()
    {
        AddChild(_directDriver);
    }

    public Task<TargetIntent> RetrieveTargetIntent(TargetIntentAcquirer method, CancellationToken ct, IReadOnlyCollection<ITargetRule>? rules = null)
    {
        return method switch
        {
            TargetIntentAcquirer.Self =>
                throw new InvalidOperationException(
                "Unexpected self target intent request, should be handled by the state machine itself."),
            
            TargetIntentAcquirer.Melee =>
                throw new System.NotImplementedException(),
            
            TargetIntentAcquirer.Direct =>
                _directDriver.RetrieveTarget(ct),
            
            TargetIntentAcquirer.Free =>
                throw new System.NotImplementedException(),

            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };
    }
}