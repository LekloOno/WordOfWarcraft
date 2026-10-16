using System;
using CombatCore;
using Godot;

namespace WowGd.Src.Combat.Statuses;

[GlobalClass]
public partial class LevelModelData : Resource
{
    [Export] private int _max = 1;
    [Export] private MergeOp _mergeOp;
    [Export] private bool _endsAtZero = false;
    [Export] private Curve? _curve = null;

    private Func<int, float>? Curve => _curve == null ? null : (i) => _curve.Sample(i);

    public LevelModel Build() => new(_max, _mergeOp, _endsAtZero, Curve);
}