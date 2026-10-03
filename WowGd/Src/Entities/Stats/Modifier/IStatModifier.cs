using System;

namespace WowGd.Src.Entities.Stats.Modifier;

public interface IStatModifier
{
    IDisposable? ApplyTo(IEntity entity, float weight);
}