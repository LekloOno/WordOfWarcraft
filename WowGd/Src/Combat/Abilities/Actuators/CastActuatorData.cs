using Godot;
using WowGd.Src.Combat.Abilities.Actuation;

namespace WowGd.Src.Combat.Abilities.Actuators;

[GlobalClass]
public partial class CastActuatorData : ActuatorData
{
    public override string Id => "cast_actuator";
    public override DeliveryMode DeliveryMode => DeliveryMode.PerWord;

    // I let the possibility to use a different exposed actuation Type.
    // I'll see later if there's no point in it, but right now, I see it as just an added flexibility. Why not.
    [Export] private ActuationType _actuationType = ActuationType.Cast;
    public override ActuationType ActuationType => _actuationType;

    [Export] public float Charge { get; private set; }= 100f;
    [Export] public float AccuracyMultiplier { get; private set; } = 0.5f;
    [Export] public float PerfectMultiplier { get; private set; } = 1f;
    [Export] public int   TargetLenght      { get; private set; } = 5;

    public override IActuator Build() =>
        new CastActuator(this);
}