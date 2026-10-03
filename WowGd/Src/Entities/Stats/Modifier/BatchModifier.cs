using System;
using System.Collections.Generic;
using Godot;

namespace WowGd.Src.Entities.Stats.Modifier;

[GlobalClass]
public partial class BatchModifier : StatModifier
{
    [Export] private Godot.Collections.Array<SingleStatModifier> _modifiers = [];

    public override IDisposable? ApplyTo(IEntity entity, float weight)
    {
        List<IDisposable> handles = [];
        foreach (SingleStatModifier val in _modifiers)
            if (val.ApplyTo(entity, weight) is IDisposable handle)
                handles.Add(handle);

        if (handles.Count == 0)
            return null;

        return new BatchModifierHandle(handles);
    }

    private sealed class BatchModifierHandle(List<IDisposable> handles) : IDisposable
    {
        List<IDisposable>? _handles = handles;
        public void Dispose()
        {
            if (_handles is null)
                return;

            foreach (IDisposable handle in _handles)
                handle.Dispose();

            _handles = null!;
            return;
        }
    }
}