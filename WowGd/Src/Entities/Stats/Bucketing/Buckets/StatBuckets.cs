using System;
using System.Numerics;

namespace WowGd.Src.Entities.Stats.Bucketing.Buckets;

public sealed class StatBuckets<TVal>(int layers, TVal identity)
    where TVal :
        IAdditionOperators<TVal, TVal, TVal>,
        ISubtractionOperators<TVal, TVal, TVal>
{
    private readonly HomogeneousBucket<TVal> _flat = new(identity);
    private readonly HomogeneousBucket<float>[] _layers = CreateLayers(layers);

    public TVal Flat => _flat.Modifier;
    public float Multiplier => LayersZeroed ? 0f : _zeroPreservedCache;
    private uint _zeroedLayers = 0;
    private bool LayersZeroed => _zeroedLayers != 0;
    private float _zeroPreservedCache = 1f;

    public IDisposable? AddFlatModifier(TVal value) =>
        _flat.AddModifier(value);

    public IDisposable? AddLayerModifier(float value, int layer)
    {
        if (layer >= layers)
            throw new IndexOutOfRangeException();

        var bucket = _layers[layer];

        float oldModifier = bucket.Modifier;

        LayerModifierHandle handle =
            new(this, bucket, bucket.AddModifier(value));
            
        float newModifier = bucket.Modifier;

        UpdateLayersCache(oldModifier, newModifier);

        return handle;
    }

    private void RemovedModifierFrom(IDisposable handle, HomogeneousBucket<float> bucket)
    {
        float oldModifier = bucket.Modifier;
        handle.Dispose();
        float newModifier = bucket.Modifier;

        UpdateLayersCache(oldModifier, newModifier);
    }

    private static HomogeneousBucket<float>[] CreateLayers(int layers)
    {
        var result = new HomogeneousBucket<float>[layers];

        for (int i = 0; i < layers; i++)
            result[i] = new(1f);

        return result;
    }

    private void UpdateLayersCache(float prev, float next)
    {
        bool wasZero = prev == 0f;
        bool isZero  = next == 0f;

        if (wasZero != isZero)
        {
            if (isZero)
            {
                _zeroPreservedCache /= prev;
                _zeroedLayers ++;
            }
            else
            {
                _zeroPreservedCache *= next;
                _zeroedLayers --;
            }
        }
        else if (!isZero)
            _zeroPreservedCache *= next / prev;
    }

    private sealed class LayerModifierHandle(StatBuckets<TVal> buckets, HomogeneousBucket<float> bucket, IDisposable handle) : IDisposable
    {
        IDisposable? _handle = handle;
        public void Dispose()
        {
            if (_handle == null)
                return;

            buckets.RemovedModifierFrom(_handle, bucket);
            _handle = null;
        }
    }
}