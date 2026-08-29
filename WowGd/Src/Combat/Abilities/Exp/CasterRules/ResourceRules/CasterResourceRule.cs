using Godot;

namespace WowGd.Src.Combat.Abilities.Exp.CasterRules.ResourceRules;

public abstract partial class CasterResourceRule : CasterRule
{
    [Export] public float               Amount      { get; private set; }
    [Export] public ResourceAmountType  AmountType  { get; private set; }
    [Export] public ResourceComparison  Comparison  { get; private set; }
}