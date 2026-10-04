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

    public override void _Ready()
    {
        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
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