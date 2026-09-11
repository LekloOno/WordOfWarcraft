using System;

namespace WowGd.Src.Input.Hands;

public interface IListenableHandInputMode
{
    event Action? InputStarted;
    event Action? InputStopped;
    event Action? InputPushed;
    event Action? InputRemoved;
}
