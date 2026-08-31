using System.Collections.Generic;
using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.CasterRules;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Data.Resources;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

namespace WowGd.Src.Combat.Abilities.Exp.Launch;

public partial class LaunchData : Resource, ILaunchData, IComponentFactory<ILaunch>
{
    [Export] public string Id { get; private set; } = string.Empty;

    [Export] private Array<CasterRule>          _casterRules    = [];
    [Export] private Array<TargetRule>          _targetRules    = [];
    [Export] private Array<EffectsHolderData>   _effectsHolders = [];

    public List<ICasterRule>        CaterRules      => [.. _casterRules];
    public List<ITargetRule>        TargetRules     => [.. _targetRules];
    public List<IEffectsHolderData> EffectsHolders  => [.. _effectsHolders];

    public ILaunch Build() =>
        new Launch(_casterRules, _targetRules, _effectsHolders);
}