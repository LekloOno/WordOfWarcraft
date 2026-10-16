using System;
using System.Numerics;
using CombatCore;

namespace WowGd.Src.Entities.Stats.Bucketing.Buckets;

public sealed class StatBuckets<TVal>(int layers, TVal identity)
    where TVal :
        IAdditionOperators<TVal, TVal, TVal>,
        ISubtractionOperators<TVal, TVal, TVal>,
        IEqualityOperators<TVal, TVal, bool>
{
    public event Action? Changed;
	private void NotifyChanged() => Changed?.Invoke();

    private readonly HomogeneousBucket<TVal> _flat = new(identity);
    private readonly HomogeneousBucket<float>[] _layers = CreateLayers(layers);

    public TVal Flat => _flat.Modifier;
    public float Multiplier => LayersZeroed ? 0f : _zeroPreservedCache;
    private uint _zeroedLayers = 0;
    private bool LayersZeroed => _zeroedLayers != 0;
    private float _zeroPreservedCache = 1f;

    public IEffectHandle AddModifier(TVal flat, float mult, int layer = -1)
    {
        FlatModifierHandle? flatHandle = flat == identity
            ? null
            : new FlatModifierHandle(this, _flat.AddModifier(flat));

        if (mult == 1f)

        if (layer >= layers)
            throw new IndexOutOfRangeException();

        var bucket = _layers[layer];

        float oldModifier = bucket.Modifier;

        LayerModifierHandle? layerHandle = mult == 1f
            ? null
            : new(this, bucket, bucket.AddModifier(mult));

        return new CombinedModifierHandle(flatHandle, layerHandle);
    }

    public IDisposable? AddFlatModifier(TVal value)
	{
        if (value == identity)
            return null;

		var inner = _flat.AddModifier(value);
		NotifyChanged();
		return new FlatModifierHandle(this, inner);
	}

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
        NotifyChanged();
        return handle;
    }

    private void RemovedModifierFrom(IDisposable handle, HomogeneousBucket<float> bucket)
    {
        float oldModifier = bucket.Modifier;
        handle.Dispose();
        float newModifier = bucket.Modifier;

        UpdateLayersCache(oldModifier, newModifier);
        NotifyChanged();
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

    private sealed class LayerModifierHandle(StatBuckets<TVal> buckets, HomogeneousBucket<float> bucket, IDisposable handle) : IEffectHandle
    {
        IDisposable? _handle = handle;
        public void Dispose()
        {
            if (_handle == null)
                return;

            buckets.RemovedModifierFrom(_handle, bucket);
            _handle = null;
        }

        public void Reweight(float weight)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FlatModifierHandle(StatBuckets<TVal> buckets, IDisposable handle) : IEffectHandle
	{
		private IDisposable? _handle = handle;

		public void Dispose()
		{
			if (_handle == null)
				return;

			_handle.Dispose();
			_handle = null;
			buckets.NotifyChanged();
		}

        public void Reweight(float weight)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class CombinedModifierHandle(FlatModifierHandle? flatHandle, LayerModifierHandle? layerHandle) : IEffectHandle
    {
        readonly FlatModifierHandle? _flatHandle = flatHandle;
        readonly LayerModifierHandle? _layerHandle = layerHandle;
        public void Dispose()
        {
            _flatHandle?.Dispose();
            _layerHandle?.Dispose();
        }

        public void Reweight(float weight)
        {
            _flatHandle?.Reweight(weight);
            _layerHandle?.Reweight(weight);
        }
    }
}