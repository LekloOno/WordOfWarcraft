using System;
using CombatCore;
using Godot;

namespace WowGd.Src.Combat.Statuses;

[GlobalClass]
public abstract partial class StatusInstall : Resource
{
    internal abstract IDisposable Build(StatusInstance inst, Authority auth);
}