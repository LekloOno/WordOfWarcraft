using System;

namespace WowGd.Src.Input.Hands;

public interface IListenableHandInputMode
{
    event Action? InputStarted;
    event Action? InputStopped;
}
