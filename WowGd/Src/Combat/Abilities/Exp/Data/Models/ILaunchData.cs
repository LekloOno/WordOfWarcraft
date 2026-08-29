using System.Collections.Generic;

namespace WowGd.Src.Combat.Abilities.Exp.Data.Models;

public interface ILaunchData
{
    string Id { get; }

    List<ICasterPreconditionData>   CasterPreconditions { get; }
    List<ITargetPreconditionData>   TargetPreconditions { get; }
    List<IEffectsHolderData>        EffectsHolders      { get; }
}