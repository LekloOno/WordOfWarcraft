using System.Threading;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Actuators;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Launch;
using WowGd.Src.Combat.Abilities.Exp.Targeting;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp;

[GlobalClass]
public partial class AbilityStateMachine : Node, IAbility
{ 
    [Export] private AbilityData _data = null!;

    private ILaunch[]       _instantLaunches    = [];
    private ILaunch[]       _targetingLaunches  = [];
    private IActuator       _actuator           = null!;
    private ILaunch[]       _actuationLaunches  = [];

    private CancellationTokenSource? _cts;

    public void Resync()
    {
        if (_actuator != null)
            _actuator.Actuated -= OnActuated;

        _instantLaunches    = _data.InstantLaunchesDt.BuildAll();
        _targetingLaunches  = _data.TargetingLaunchesDt.BuildAll();
        _actuator           = _data.ActuatorDt.Build();
        _actuationLaunches  = _data.ActuationLaunchesDt.BuildAll();

        _actuator.Actuated += OnActuated;
    }

    public override void _Ready()
    {
        Resync();
    }

    public bool Stop()
    {
        if (_cts is null)
            return false;

        _cts?.Cancel();
        return true;
    }

    public async void Start(IEntity caster)
    {
        _cts?.Cancel();

        // STEP 1 - check preconditions
        if (!_data.StartPreconditionsDt.CheckAll(caster))
            return;

        // STEP 2 - trgger instant launches
        _instantLaunches.LaunchAll(new(caster, new(caster), 1f));

        _cts = new CancellationTokenSource();

        // STEP 3 - retrieve target intent
        TargetIntent targetIntent;
        do
        {
            targetIntent =  await
                caster.TargetIntentDriver.RetrieveTargetIntent(
                    _data.TargetIntentAcquirer,
                    _cts.Token,
                    _data.TargetRules);
        }
        while (!_data.TargetRulesDt.CheckAll(caster, targetIntent));

        // STEP 4 - trigger targeting launches
        _targetingLaunches.LaunchAll(new(caster, targetIntent, 1f));

        // STEP 5 - wait for actuation completion
        await _actuator.Actuate(caster, targetIntent, _cts.Token);

        // STEP 5.1, 5.2, 5.3 and 5.4 in OnActuated handler
    }

    private void OnActuated(ActuatePayload payload)
    {
        // STEP 5.1 - trigger actuation launches
        _actuationLaunches.LaunchAll(payload);

        // STEP 5.2, 5.3 and 5.4 - check if current state still passes through looping conditions
        if (!_data.LoopRulesDt.CheckAll(payload) ||
            !_data.CasterLoopRulesDt.CheckAll(payload.Caster) ||
            !_data.TargetLoopRulesDt.CheckAll(payload.Caster, payload.Intent))
        {
            Stop();
            return;
        }
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
}