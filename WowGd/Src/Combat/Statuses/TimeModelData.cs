using CombatCore;
using Godot;

namespace WowGd.Src.Combat.Statuses;

[GlobalClass]
public partial class TimeModelData : Resource
{
    [Export] private MergeOp _mergeOp;
    public TimeModel Build() => new(_mergeOp);
}