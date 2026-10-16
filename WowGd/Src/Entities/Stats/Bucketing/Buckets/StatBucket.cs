using System;
using System.Collections.Generic;
using CombatCore;

namespace WowGd.Src.Entities.Stats.Bucketing.Buckets;

public partial class StatBucket<TIn, TOut>(
    Func<TOut, TIn, TOut> addOp,
    Func<TOut, TIn, TOut> subOp,
    TOut identity
) {
    private readonly Dictionary<ModifierHandle, TIn> _bucket = [];
    public TOut Modifier { get; private set; } = identity;

    public IEffectHandle AddModifier(TIn value)
    {
        ModifierHandle handle = new(this);
        _bucket.Add(handle, value);
        Modifier = addOp(Modifier, value);
        return handle;
    }

    private void RemoveModifier(ModifierHandle handle)
    {
        if (_bucket.Remove(handle, out var value))
            Modifier = subOp(Modifier, value);
    }

    private sealed class ModifierHandle(StatBucket<TIn, TOut> statBucket) : IEffectHandle
    {
        private StatBucket<TIn, TOut> _statBucket = statBucket;

        public void Dispose()
        {
            if (_statBucket == null)
                return;

            _statBucket.RemoveModifier(this);
            _statBucket = null!;
        }

        public void Reweight(float weight)
        {
            throw new NotImplementedException();
        }
    }
}