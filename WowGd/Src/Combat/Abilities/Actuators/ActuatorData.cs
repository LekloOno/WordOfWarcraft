using Godot;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Actuators;

[GlobalClass]
public abstract partial class ActuatorData : Resource, IActuatorData, IComponentFactory<IActuator>
{
    [Export] private int _minWordLength = 0;
    [Export] private int _maxWordLength = 15;
    [Export] private int _lookahead = 10;
    [Export] private ActuatorFlavor? _flavor;

    public abstract DeliveryMode DeliveryMode { get; }
    public abstract ActuationType ActuationType { get; }

    public abstract string Id { get; }
    public abstract IActuator Build();

    public WordRequest BuildRequest() =>
        new(DeliveryMode, ActuationType, _minWordLength, _maxWordLength, _lookahead, _flavor?.Flavor ?? "");
}