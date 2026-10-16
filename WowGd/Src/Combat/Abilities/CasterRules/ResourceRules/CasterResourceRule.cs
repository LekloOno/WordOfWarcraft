using Godot;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.CasterRules.ResourceRules;

public abstract partial class CasterResourceRule<T> : CasterRule where T : IStandardResource
{
    [Export] public float               Amount      { get; private set; }
    [Export] public ResourceAmountType  AmountType  { get; private set; }
    [Export] public ResourceComparison  Comparison  { get; private set; }

    public override bool Check(IEntity caster)
    {
        if (!caster.ResourceManager.TryGetResource(ResourceRegistry.GetId<T>(), out IStandardResource? res))
            return false;

        return this.Compare(res.Current, res.Max, res.Base);
    }
}