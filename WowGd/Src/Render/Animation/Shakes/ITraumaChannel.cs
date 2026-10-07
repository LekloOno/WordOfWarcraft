using System;

namespace WowGd.Src.Render.Animation.Shakes;

public interface ITraumaChannel : IDisposable
{
    void Add(float amount);
    void RaiseTo(float level);

    float Intensity { get; }
    bool Active { get; }
}