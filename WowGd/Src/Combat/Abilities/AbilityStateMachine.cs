using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CombatCore;
using CombatCore.Sources;
using Godot;
using WowGd.Src.Combat.Abilities.Actuation;
using WowGd.Src.Combat.Abilities.Actuators;
using WowGd.Src.Combat.Abilities.Behaviors;
using WowGd.Src.Combat.Abilities.Behaviors.Data;
using WowGd.Src.Combat.Abilities.CoolDowns;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Launch;
using WowGd.Src.Combat.Abilities.Spells;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities;

[GlobalClass]
public partial class AbilityStateMachine : Node,
    IAbility,
    IActuableAbility,
    ICancelLaunchable,
    ICoolDownable,
    IInstantLaunchableAbility,
    IStopLaunchable,
    ITargetLaunchable
{
    private static SpellBook? _spellBook;
    [Export] private AbilitySmData _data = null!;
    private static uint _count;
    private readonly uint _id = _count ++; 

    public IActuableData ActuableData => _data;
    public ICancelLaunchableData CancelLaunchableData => _data;
    public IInstantLaunchableData InstantLaunchableData => _data;
    public IStopLaunchableData StopLaunchableData => _data;
    public ITargetLaunchableData TargetLaunchableData => _data;
    private ICoolDown _coolDown = null!;
    public ICoolDown CoolDown => _coolDown;
    private SpellInstance? _currentInstance;

    public IAbilityData Data => _data;
    private ILaunch[]       _instantLaunches    = [];
    private ILaunch[]       _targetingLaunches  = [];
    private IActuator?       _actuator;
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
        _actuator           = _data.ActuatorDt?.Build();
        _actuationLaunches  = _data.ActuationLaunchesDt.BuildAll();
        _stopLaunches       = _data.StopLaunchesDt.BuildAll();
        _cancelLaunches     = _data.CancelLaunchesDt.BuildAll();
        _coolDown           = new CoolDown(_data);

        if (_actuator != null)
            _actuator.Actuated += OnActuated;
    }

    public override void _Ready()
    {
        Resync();
    }

    public bool Cancel(IEntity caster)
    {
        _cts?.Cancel();
        return true;
    }

    private bool CanEnd() => _enabled && _cts is not null;

    private void Cleanup(StopReason reason)
    {
        _cts?.Cancel();
        _cts = null;

        _currentInstance?.Stop(reason);
        _currentInstance = null;
    }

    private void Stop(ActuatePayload payload, StopReason reason)
    {
        if (!CanEnd())
            return;

        _stopLaunches.LaunchAll(payload, _currentInstance!.Node);

        Stopped?.Invoke();
        StopLaunchesEmitted?.Invoke(_stopLaunches);

        Cleanup(reason);
    }

    public async Task<bool> Start(IEntity caster)
    {
        if (!_enabled)
            return false;

        if (!CoolDown.Completed())
            return false;

        _cts?.Cancel();
        // STEP 1 - check preconditions
        if (!_data.StartPreconditionsDt.CheckAll(caster))
            return false;

        _spellBook ??= new(CombatCoreNode.Authority);
        _currentInstance = _spellBook.Start(_id, caster.Id);

        Started?.Invoke();

        // STEP 2 - trgger instant launches
        _instantLaunches.LaunchAll(new(caster, new(caster), 1f), _currentInstance!.Node);
        InstantLaunchesEmitted?.Invoke(_instantLaunches);

        _cts = new CancellationTokenSource();
        using var controller = new TargetIntentController(
            caster, _data.TargetIntentAcquirer, _data.TargetRulesDt, default, _cts.Token);

        try
        {
            controller.RequestRefresh();
            TargetingStarted?.Invoke();

            TargetResult result = await controller.WaitForValidTargetAsync(_cts.Token);
            if (!result.TryGet(out TargetIntent targetIntent))
            {
                TargetingFailed?.Invoke(result.Failure!.Value);
                _cts.Cancel();
                return false;
            }
            
            TargetingCompleted?.Invoke();

            CoolDown.StartCd();
            
            // STEP 4 - trigger targeting launches
            _targetingLaunches.LaunchAll(new(caster, targetIntent, 1f), _currentInstance!.Node);
            TargetLaunchesEmitted?.Invoke(_targetingLaunches);

            // STEP 5 - wait for actuation completion
            if (_actuator != null)
            {
                ActuationStarted?.Invoke();
                bool completed = await _actuator.Actuate(caster, controller, _cts.Token);
                if (!completed)
                    return false;
                ActuationCompleted?.Invoke();
            }

            // STEP 5.1, 5.2, 5.3 and 5.4 in OnActuated handler
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
        {
            _cancelLaunches.LaunchAll(new (caster, new(caster), 1f), _currentInstance!.Node);

            Cancelled?.Invoke();
            CancelLaunchesEmitted?.Invoke(_cancelLaunches);

            Cleanup(StopReason.Cancelled);
            return false; 
        }

        Cleanup(StopReason.Completed);
        return true;
    }

    private void OnActuated(ActuatePayload payload)
    {
        if (!_enabled)
            return;
            
        // STEP 5.1 - trigger actuation launches
        _actuationLaunches.LaunchAll(payload, _currentInstance!.Node);
        ActuationLaunchesEmitted?.Invoke(_actuationLaunches);

        // STEP 5.2, 5.3 and 5.4 - check if current state still passes through looping conditions
        if (!_data.LoopRulesDt.CheckAll(payload) ||
            !_data.CasterLoopRulesDt.CheckAll(payload.Caster) ||
            !_data.TargetLoopRulesDt.CheckAll(payload.Caster, payload.Intent))
        {
            Stop(payload, StopReason.RuleViolation);
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

    public event Action? Started;
    public event Action? Stopped;
    public event Action? Cancelled;
    public event Action? TargetingStarted;
    public event Action? TargetingCompleted;
    public event Action<TargetFailure>? TargetingFailed;
    public event Action<IEnumerable<ILaunch>>? ActuationLaunchesEmitted;
    public event Action? ActuationStarted;
    public event Action? ActuationCompleted;
    public event Action<IEnumerable<ILaunch>>? CancelLaunchesEmitted;
    public event Action<IEnumerable<ILaunch>>? InstantLaunchesEmitted;
    public event Action<IEnumerable<ILaunch>>? StopLaunchesEmitted;
    public event Action<IEnumerable<ILaunch>>? TargetLaunchesEmitted;
}