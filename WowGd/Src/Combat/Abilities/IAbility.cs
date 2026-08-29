using System;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities;

public interface IAbility : IDisablable
{
    bool Start();
    bool Stop();

    event Action? Started;
    event Action? Stopped;
}