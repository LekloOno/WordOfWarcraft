using System.Collections.Generic;

namespace WowGd.Src.Combat.Abilities.Data;

public interface ILaunchData
{
    string Id { get; }

    List<ICasterRule>           CaterRules      { get; }
    List<ITargetRule>           TargetRules     { get; }
    List<IEffectsHolderData>    EffectsHolders  { get; }
}