using System;
using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public sealed class TraumaChannel(ITraumaSettings settings, ITraumaSamplerSettings samplerSettings, Action<TraumaChannel> release) : ITraumaChannel
{
    public ITraumaSamplerSettings SamplerSettings => samplerSettings;
    private readonly ITraumaSettings _settings = settings;
    private readonly Action<TraumaChannel> _release = release;

    private float _trauma;
    public float Intensity => _trauma * _trauma;
    public bool Active => _trauma > 0f;

    public void Add(float amount) => _trauma = Mathf.Min(_trauma + amount, _settings.Cap);
    public void RaiseTo(float level) => _trauma = Mathf.Clamp(level, _trauma, _settings.Cap);
    public void Tick(float delta) => _trauma = Mathf.Max(0f, _trauma - delta * _settings.DecayRate);
    public void Dispose() => _release(this);
}