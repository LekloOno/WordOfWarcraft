using System.Collections.Generic;
using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.Actuators;
using WowGd.Src.Combat.Abilities.Exp.CasterRules;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Launch;
using WowGd.Src.Combat.Abilities.Exp.LoopRules;
using WowGd.Src.Combat.Abilities.Exp.Targeting.TargetRules;

namespace WowGd.Src.Combat.Abilities.Exp;

public partial class AbilityData : Resource, IAbilityData
{
    [Export] public string Id { get; private set; } = string.Empty;
    [Export] public Array<CasterRule> StartPreconditionsDt  = [];
    [Export] public Array<LaunchData> InstantLaunchesDt     = [];
    [Export] public Array<TargetRule> TargetRulesDt         = [];
    [Export] public TargetIntentAcquirer TargetIntentAcquirer { get; private set; }
    [Export] public Array<LaunchData> TargetingLaunchesDt   = [];
    [Export] public ActuatorData      ActuatorDt            = null!;
    [Export] public Array<LaunchData> ActuationLaunchesDt   = [];
    [Export] public Array<LoopRule>   LoopRulesDt           = [];
    [Export] public Array<CasterRule> CasterLoopRulesDt     = [];
    [Export] public Array<TargetRule> TargetLoopRulesDt     = [];
    [Export] public Array<LaunchData> StopLaunchesDt        = [];
    [Export] public Array<LaunchData> CancelLaunchesDt      = [];


    public IReadOnlyList<ICasterRule> StartPreconditions    => [.. StartPreconditionsDt];
    public IReadOnlyList<ILaunchData> InstantLaunches       => [.. InstantLaunchesDt];
    public IReadOnlyList<ITargetRule> TargetRules           => [.. TargetRulesDt];
    public IReadOnlyList<ILaunchData> TargetingLaunches     => [.. TargetingLaunchesDt];
    public IActuatorData              Actuator              => ActuatorDt;
    public IReadOnlyList<ILaunchData> ActuationLaunches     => [.. ActuationLaunchesDt];
    public IReadOnlyList<ILoopRule>   LoopRules             => [.. LoopRulesDt];
    public IReadOnlyList<ICasterRule> CasterLoopRules       => [.. CasterLoopRulesDt];
    public IReadOnlyList<ITargetRule> TargetLoopRules       => [.. TargetLoopRulesDt];
    public IReadOnlyList<ILaunchData> StopLaunches          => [.. StopLaunchesDt];
    public IReadOnlyList<ILaunchData> CancelLaunches        => [.. CancelLaunchesDt];
}