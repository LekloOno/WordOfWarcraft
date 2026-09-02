using Godot;
using WowGd.Src.Combat.Abilities.Actuation;

namespace WowGd.Src.Combat.Abilities.Actuators;

[GlobalClass]
public partial class WordActuatorData : ActuatorData
{
    public override string Id => "cast_actuator";
    public override DeliveryMode DeliveryMode => DeliveryMode.PerWord;

    // Another type can be used, for example
    [Export] private ActuationType _actuationType = ActuationType.Cast;
    public override ActuationType ActuationType => _actuationType;

    [Export] public float AccuracyMultiplier { get; private set; } = 0f;
    [Export] public float PerfectMultiplier { get; private set; } = 1f;

    public override IActuator Build() =>
        new WordActuator(this);
}