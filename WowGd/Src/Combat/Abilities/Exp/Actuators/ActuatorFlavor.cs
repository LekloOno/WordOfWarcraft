using Godot;

namespace WowGd.Src.Combat.Abilities.Exp.Actuators;

[GlobalClass]
public partial class ActuatorFlavor : Resource
{
    [Export] public string Flavor { get; private set; } = string.Empty;
}