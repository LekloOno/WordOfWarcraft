using System;
using System.Threading;
using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Actuators;

public class CastActuator(CastActuatorData data) : IActuator
{
    private readonly CastActuatorData _data = data;
    private float _currentCharge = 0f;

    public event Action<ActuatePayload>? Actuated;

    public async Task<bool> Actuate(IEntity entity, TargetIntentController intentController, CancellationToken ct)
    {
        _currentCharge = 0f;

        try
        {
            await foreach (TypingPackage package in entity.ActuatorDriver.StreamTypingAsync(_data.BuildRequest(), ct))
            {
                float weightMod = package.GetWeight(_data.PerfectMultiplier, _data.AccuracyMultiplier);
                weightMod *= (float) package.CorrectCharacters / _data.TargetLenght;
                
                _currentCharge += weightMod * package.CorrectCharacters;

                if (_currentCharge >= _data.Charge)
                {
                    TargetResult result = await intentController.WaitForValidTargetAsync(ct);
                    if (!result.TryGet(out TargetIntent intent))
                        return false;
                        
                    Actuated?.Invoke(new ActuatePayload(entity, intent, 1f));
                    _currentCharge -= _data.Charge;
                }
            }
            return true;
        }
        catch (OperationCanceledException) { return false; }
    }
}