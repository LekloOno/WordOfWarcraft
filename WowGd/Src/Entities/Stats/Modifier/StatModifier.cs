using System;
using Godot;

namespace WowGd.Src.Entities.Stats.Modifier;

// Just for godot serialization structure
[GlobalClass]
public abstract partial class StatModifier : Resource, IStatModifier
{
    public abstract IDisposable? ApplyTo(IEntity entity, float weight);
}