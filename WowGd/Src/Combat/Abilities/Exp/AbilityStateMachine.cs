using Godot;
using WowGd.Src.Combat.Abilities.Exp.Actuators;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Launch;

namespace WowGd.Src.Combat.Abilities.Exp;

[GlobalClass]
public partial class AbilityStateMachine : Node, IAbility
{ 
    [Export] private AbilityData _data = null!;

    private ILaunch[]       _instantLaunches    = [];
    private ILaunch[]       _targetingLaunches  = [];
    private IActuator       _actuator           = null!;
    private ILaunch[]       _actuationLaunches  = [];

    public void Resync()
    {
        foreach(Node child in GetChildren())
            child.QueueFree();

        _instantLaunches    = _data.InstantLaunchesDt.SyncAll(this);
        _targetingLaunches  = _data.TargetingLaunchesDt.SyncAll(this);
        _actuator           = _data.ActuatorDt.Build();
        _actuationLaunches  = _data.ActuationLaunchesDt.SyncAll(this);
    }

    public override void _Ready()
    {
        Resync();
    }

    public bool Enabled => throw new System.NotImplementedException();

    public void AttachExternalLaunch(ILaunch launch, AbilityLaunchHook hook)
    {
        throw new System.NotImplementedException();
    }

    public bool Disable()
    {
        throw new System.NotImplementedException();
    }

    public bool Enable()
    {
        throw new System.NotImplementedException();
    }

    public bool Start()
    {
        throw new System.NotImplementedException();
    }

    public bool Stop()
    {
        throw new System.NotImplementedException();
    }
}