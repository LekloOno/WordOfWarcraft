using System.Collections.Generic;
using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.CasterRules;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Data.Resources;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

namespace WowGd.Src.Combat.Abilities.Exp.Launch;

[GlobalClass]
public partial class LaunchData : Resource, ILaunchData, IComponentFactory<ILaunch>
{
    [Export] public string Id { get; private set; } = string.Empty;

    [Export] public Array<CasterRule>           CasterRulesDt    { get; private set; } = [];
    [Export] public Array<TargetRule>           TargetRulesDt    { get; private set; } = [];
    [Export] public Array<EffectsHolderData>    EffectsHoldersDt { get; private set; } = [];

    public List<ICasterRule>        CaterRules      => [.. CasterRulesDt];
    public List<ITargetRule>        TargetRules     => [.. TargetRulesDt];
    public List<IEffectsHolderData> EffectsHolders  => [.. EffectsHoldersDt];

    public ILaunch Build() => new Launch(this);
}