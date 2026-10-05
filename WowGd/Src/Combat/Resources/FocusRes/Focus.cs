using System;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Stats.Focus;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Resources.FocusRes;

[GlobalClass]
public partial class Focus : Node, IFocus
{
    private IEntity _entity = null!;
    public int Base => _entity.FocusBase();
    public int Max => _entity.Focus();
    public int Current { get; private set; }

    public event Action<int>? Consumed;
    public event Action<int>? Generated;
    public event Action<int>? MaxChanged;

    public override async void _Ready()
    {
        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;

        await _entity.Initialization;
        _entity.Statistics.GetStatistic(Entities.Stats.StatEnum.Focus).Changed += OnMaxChanged;
    }

    public override async void _EnterTree()
    {
        if (_entity == null) return;

        await _entity.Initialization;
        _entity.Statistics.GetStatistic(Entities.Stats.StatEnum.Focus).Changed += OnMaxChanged;
    }

    public override void _ExitTree()
    {
        _entity.Statistics.GetStatistic(Entities.Stats.StatEnum.Focus).Changed -= OnMaxChanged;
    }

    private void OnMaxChanged(int max)
    {
        Current = Math.Min(max, Current);
        MaxChanged?.Invoke(max);
    }

    public bool Consume(int fp, out int overflow)
    {
        int consumed = Math.Min(fp, Current);
        overflow = fp - consumed;
        Current -= consumed;

        Consumed?.Invoke(consumed);

        return true;
    }

    public bool Generate(int fp, out int overflow)
    {
        int generated = Math.Min(fp, Max - Current);
        overflow = fp - generated;
        Current += generated;

        Generated?.Invoke(generated);
    
        return true;
    }
}