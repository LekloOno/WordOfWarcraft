using System.Collections.Generic;

namespace WowGd.Src.Combat.Abilities.Exp.Data;

public interface IEffectsHolderData
{
    string              Id          { get; }
    ITriggerData        Trigger     { get; }
    IGathererData       Gatherer    { get; }
    List<IEffectData>   Effects     { get; }
}