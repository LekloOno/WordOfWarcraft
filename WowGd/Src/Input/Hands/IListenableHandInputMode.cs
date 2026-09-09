using System;

namespace WowGd.Src.Input.Hands;

public interface IListenableHandInputMode
{
    event Action? InputStart;
    event Action? InputStop;
}
