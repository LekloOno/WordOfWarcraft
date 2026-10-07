using System;
using System.Collections.Generic;
using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public partial class ReceiverShakeOutput(ITraumaSettings baseChannelSettings, ITraumaSamplerSettings baseChannelSamplerSettings)
    : Node, ITraumaReceiver, IShakeOutput, ITraumaChannel
{
    public ITraumaChannel BaseChannel => _baseChannel;
    public event Action<Vector3>? Shaken;
    public float Intensity => _baseChannel.Intensity;
    public bool Active => _baseChannel.Active;

    private readonly List<TraumaChannel> _channels = [];
    private readonly TraumaChannel _baseChannel = new(baseChannelSettings, baseChannelSamplerSettings, (c) => {});
    private float _acc;

    public ITraumaChannel? OpenChannel(ITraumaSettings settings, ITraumaSamplerSettings samplerSettings)
    {
        TraumaChannel channel = new(settings, samplerSettings, (c) => _channels.Remove(c));
        _channels.Add(channel);
        return channel;
    }

    public override void _Process(double delta)
    {
        float dt = (float) delta;

        _acc += dt;
        _baseChannel.Tick(dt);

        Vector3 shake3d = TraumaShakeSampling.Sample3D(_baseChannel, _acc);
        foreach (TraumaChannel channel in _channels)
        {
            channel.Tick(dt);
            shake3d += TraumaShakeSampling.Sample3D(channel, _acc);
        }

        Shaken?.Invoke(shake3d);
    }

    public void Add(float amount) => _baseChannel.Add(amount);
    public void RaiseTo(float level) => _baseChannel.RaiseTo(level);
}