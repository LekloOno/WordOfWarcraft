using System;
using Godot;

namespace WowGd.Src.Combat.Resources.FocusRes;

[GlobalClass]
public partial class Focus : Node, IFocus
{
    [Export] public int BaseFocus { get; private set; } = 50;
    /// <summary>
    /// Later used with possible modifiers. For now, keep it simple.
    /// </summary>
    public int MaxFocus => BaseFocus;
    public int CurrentFocus { get; private set; }

    public event Action<int>? Consumed;
    public event Action<int>? Generated;

    public bool Consume(int fp, out int overflow)
    {
        int consumed = Math.Min(fp, CurrentFocus);
        overflow = fp - consumed;
        CurrentFocus -= consumed;

        Consumed?.Invoke(consumed);

        return true;
    }

    public bool Generate(int fp, out int overflow)
    {
        int generated = Math.Min(fp, BaseFocus - CurrentFocus);
        overflow = fp - generated;
        CurrentFocus += generated;

        Generated?.Invoke(generated);
    
        return true;
    }
}