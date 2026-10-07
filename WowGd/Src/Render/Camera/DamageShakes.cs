using System;
using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Render.Animation.Shakes;
using WowGd.Src.Tools.Curve;

namespace WowGd.Src.Render.Camera;

public sealed class DamageShakes : IEntityHealthHandler, IDisposable
{
    public CurveSampler<float> _curveSampler;
    public IEntityHealth _health;
    public ITraumaChannel _traumaChannel;

    public DamageShakes(CurveSampler<float> curveSampler, IEntityHealth health, ITraumaChannel traumaChannel)
    {
        _curveSampler = curveSampler;
        _health = health;
        _traumaChannel = traumaChannel;

        this.Bind(health);
    }

    public void Dispose() => this.Unbind(_health);

    public void OnConsumed(int fp)
    {
        float damageScale = (float)fp / _health.Max;
        float trauma = _curveSampler.Sample(damageScale);
        _traumaChannel.Add(trauma);
    }

    public void OnDied() { }
    public void OnGenerated(int fp) { }
    public void OnResurrected(int hp) { }
}