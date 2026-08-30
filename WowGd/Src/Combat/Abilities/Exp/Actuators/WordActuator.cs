using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Actuators;

[GlobalClass]
public partial class WordActuator : Node, IActuator
{
    [Export] private WordActuatorData _data = null!;

    public WordActuator() {}
    public WordActuator(WordActuatorData data) {_data = data;}

    public event Action<ActuatePayload>? Actuated;

    public async Task Actuate(IEntity entity, TargetIntent intent, CancellationToken ct)
    {
        try
        {
            await foreach (TypingPackage package in entity.ActuatorDriver.StreamTypingAsync(_data.BuildRequest(), ct))
            {
                float weightMod = package.GetWeight(_data.PerfectMultiplier, _data.AccuracyMultiplier);
                Actuated?.Invoke(new ActuatePayload(entity, intent, weightMod));
            }
        }
        catch (OperationCanceledException) {}
    }
}