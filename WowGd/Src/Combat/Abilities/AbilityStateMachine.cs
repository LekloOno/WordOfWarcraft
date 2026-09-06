using System;
using System.Threading;
using Godot;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Actuators;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Launch;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities;

[GlobalClass]
public partial class AbilityStateMachine : Node, IAbility
{ 
    [Export] private AbilityData _data = null!;

    private ILaunch[]       _instantLaunches    = [];
    private ILaunch[]       _targetingLaunches  = [];
    private IActuator       _actuator           = null!;
    private ILaunch[]       _actuationLaunches  = [];
    private ILaunch[]       _stopLaunches       = [];
    private ILaunch[]       _cancelLaunches     = [];

    private CancellationTokenSource? _cts;

    public bool Enabled => _enabled;
    private bool _enabled;

    public void Resync()
    {
        if (_actuator != null)
            _actuator.Actuated -= OnActuated;

        _instantLaunches    = _data.InstantLaunchesDt.BuildAll();
        _targetingLaunches  = _data.TargetingLaunchesDt.BuildAll();
        _actuator           = _data.ActuatorDt.Build();
        _actuationLaunches  = _data.ActuationLaunchesDt.BuildAll();
        _stopLaunches       = _data.StopLaunchesDt.BuildAll();
        _cancelLaunches     = _data.CancelLaunchesDt.BuildAll();

        _actuator.Actuated += OnActuated;
    }

    public override void _Ready()
    {
        Resync();
    }

    public bool Cancel(IEntity caster)
    {
        if (!_enabled)
            return false;
            
        if (_cts is null)
            return false;

        _cts?.Cancel();
        _cancelLaunches.LaunchAll(new (caster, new(caster), 1f));
        return true;
    }

    private void Stop(ActuatePayload payload)
    {
        if (!_enabled)
            return;

        if (_cts is null)
            return;

        _cts?.Cancel();
        _stopLaunches.LaunchAll(payload);
    }

    private TargetIntentController? _targetIntentController;
    public async void Start(IEntity caster)
    {
        if (!_enabled)
            return;
            
        _cts?.Cancel();
        // STEP 1 - check preconditions
        if (!_data.StartPreconditionsDt.CheckAll(caster))
            return;

        // STEP 2 - trgger instant launches
        _instantLaunches.LaunchAll(new(caster, new(caster), 1f));

        _cts = new CancellationTokenSource();

        try
        {
            _targetIntentController = new(
                caster, 
                _data.TargetIntentAcquirer,
                _data.TargetRulesDt,
                default,
                _cts.Token);

            _targetIntentController.RequestRefresh();
            TargetIntent targetIntent = await _targetIntentController.WaitForValidTargetAsync(_cts.Token);
            
            // STEP 4 - trigger targeting launches
            _targetingLaunches.LaunchAll(new(caster, targetIntent, 1f));

            // STEP 5 - wait for actuation completion
            await _actuator.Actuate(caster, _targetIntentController, _cts.Token);

            // STEP 5.1, 5.2, 5.3 and 5.4 in OnActuated handler
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
        {
            // .. silence it   
        }
    }

    private void OnActuated(ActuatePayload payload)
    {
        if (!_enabled)
            return;
            
        // STEP 5.1 - trigger actuation launches
        _actuationLaunches.LaunchAll(payload);

        // STEP 5.2, 5.3 and 5.4 - check if current state still passes through looping conditions
        if (!_data.LoopRulesDt.CheckAll(payload) ||
            !_data.CasterLoopRulesDt.CheckAll(payload.Caster) ||
            !_data.TargetLoopRulesDt.CheckAll(payload.Caster, payload.Intent))
        {
            Stop(payload);
            return;
        }
    }    

    public void AttachExternalLaunch(ILaunch launch, AbilityLaunchHook hook)
    {
        throw new System.NotImplementedException();
    }

    public bool Disable() =>
        DisableExt.IndempDisable(ref _enabled, () => _cts?.Cancel());

    public bool Enable() =>
        DisableExt.IndempEnable(ref _enabled, () => {});
}