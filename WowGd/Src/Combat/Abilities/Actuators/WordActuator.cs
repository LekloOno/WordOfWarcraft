using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Actuators;

public class WordActuator(WordActuatorData data) : IActuator
{
    private readonly WordActuatorData _data = data;
    public event Action<ActuatePayload>? Actuated;

    public async Task<bool> Actuate(IEntity entity, TargetIntentController intentController, CancellationToken ct)
    {
        try
        {
            await foreach (TypingPackage package in entity.ActuatorDriver.StreamTypingAsync(_data.BuildRequest(), ct))
            {
                TargetResult result = await intentController.WaitForValidTargetAsync(ct);
                
                if (!result.TryGet(out TargetIntent intent))
                    return false;
                
                float weightMod = package.GetWeight(_data.PerfectMultiplier, _data.AccuracyMultiplier);
                weightMod *= (float) package.CorrectCharacters / _data.TargetLenght;
                
                Actuated?.Invoke(new ActuatePayload(entity, intent, weightMod));
            }
            return true;
        }
        catch (OperationCanceledException) { return false; }
    }
}