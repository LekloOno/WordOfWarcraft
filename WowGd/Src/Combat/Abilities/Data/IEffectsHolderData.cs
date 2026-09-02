using System.Collections.Generic;

namespace WowGd.Src.Combat.Abilities.Data;

public interface IEffectsHolderData
{
    string              Id          { get; }
    ITriggerData        Trigger     { get; }
    IGathererData       Gatherer    { get; }
    List<IEffect>   Effects     { get; }
}