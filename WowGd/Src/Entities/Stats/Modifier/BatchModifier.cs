using System.Collections.Generic;
using CombatCore;
using Godot;

namespace WowGd.Src.Entities.Stats.Modifier;

[GlobalClass]
public partial class BatchModifier : StatModifier
{
    [Export] private Godot.Collections.Array<SingleStatModifier> _modifiers = [];

    public override IEffectHandle? ApplyTo(IEntity entity, float weight)
    {
        List<IEffectHandle> handles = [];
        foreach (SingleStatModifier val in _modifiers)
            if (val.ApplyTo(entity, weight) is IEffectHandle handle)
                handles.Add(handle);

        if (handles.Count == 0)
            return null;

        return new BatchModifierHandle(handles);
    }

    private sealed class BatchModifierHandle(List<IEffectHandle> handles) : IEffectHandle
    {
        List<IEffectHandle>? _handles = handles;
        public void Dispose()
        {
            if (_handles is null)
                return;

            foreach (IEffectHandle handle in _handles)
                handle.Dispose();

            _handles = null!;
            return;
        }

        public void Reweight(float weight)
        {
            if (_handles is null)
                return;

            foreach (IEffectHandle handle in _handles)
                handle.Reweight(weight);
        }
    }
}