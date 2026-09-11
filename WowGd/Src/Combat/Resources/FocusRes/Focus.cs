using System;
using Godot;

namespace WowGd.Src.Combat.Resources.FocusRes;

[GlobalClass]
public partial class Focus : Node, IFocus
{
    [Export] public int Base { get; private set; } = 50;
    /// <summary>
    /// Later used with possible modifiers. For now, keep it simple.
    /// </summary>
    public int Max => Base;
    public int Current { get; private set; }

    public event Action<int>? Consumed;
    public event Action<int>? Generated;

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
        int generated = Math.Min(fp, Base - Current);
        overflow = fp - generated;
        Current += generated;

        Generated?.Invoke(generated);
    
        return true;
    }
}