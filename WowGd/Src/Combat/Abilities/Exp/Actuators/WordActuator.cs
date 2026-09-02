using System;
using System.Threading;
using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Actuators;

public class WordActuator(WordActuatorData data) : IActuator
{
    private readonly WordActuatorData _data = data;
    public event Action<ActuatePayload>? Actuated;

    public async Task Actuate(IEntity entity, TargetIntentController intentController, CancellationToken ct)
    {
        try
        {
            await foreach (TypingPackage package in entity.ActuatorDriver.StreamTypingAsync(_data.BuildRequest(), ct))
            {
                TargetIntent intent = await intentController.WaitForValidTargetAsync(ct);
                
                float weightMod = package.GetWeight(_data.PerfectMultiplier, _data.AccuracyMultiplier);
                Actuated?.Invoke(new ActuatePayload(entity, intent, weightMod));
            }
        }
        catch (OperationCanceledException) {}
    }
}