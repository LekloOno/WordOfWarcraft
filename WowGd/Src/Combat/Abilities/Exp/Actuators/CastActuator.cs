using System;
using System.Threading;
using System.Threading.Tasks;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Actuators;

public class CastActuator(CastActuatorData data) : IActuator
{
    private readonly CastActuatorData _data = data;
    private float _currentCharge = 0f;

    public event Action<ActuatePayload>? Actuated;

    public async Task Actuate(IEntity entity, TargetIntentController intentController, CancellationToken ct)
    {
        _currentCharge = 0f;

        try
        {
            await foreach (TypingPackage package in entity.ActuatorDriver.StreamTypingAsync(_data.BuildRequest(), ct))
            {
                float weightMod = package.GetWeight(_data.PerfectMultiplier, _data.AccuracyMultiplier);
                _currentCharge += weightMod * package.CorrectCharacters;

                if (_currentCharge >= _data.Charge)
                {
                    TargetIntent intent = await intentController.WaitForValidTargetAsync(ct);
                    Actuated?.Invoke(new ActuatePayload(entity, intent, 1f));
                    _currentCharge -= _data.Charge;
                }
            }
        }
        catch (OperationCanceledException) {}
    }
}