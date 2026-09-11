using System;

namespace WowGd.Src.Combat.Resources.FocusRes;

public interface IFocus
{
    int BaseFocus       { get; }
    int MaxFocus        { get; }
    int CurrentFocus    { get; }

    event Action<int>? Consumed;
    event Action<int>? Generated;

    bool Consume(int fp, out int overflow);
    bool Generate(int fp, out int overflow);
}