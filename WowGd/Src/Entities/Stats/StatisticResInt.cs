using Godot;

namespace WowGd.Src.Entities.Stats;

[GlobalClass, Tool]
public partial class StatisticResInt : StatisticRes<int>
{
    public StatisticResInt() : base() {}
    public StatisticResInt(int @base) : base(@base) {}

    [Export]
    public int Base
    {
        get => GetBase;
        set => SetBase(value);
    }
}