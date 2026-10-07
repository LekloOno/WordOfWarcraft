using System;

namespace WowGd.Src.Render.Animation.Shakes;

[Flags]
public enum ShakeSpatialChannels
{
    X = 1 << 0,
    Y = 1 << 1,
    Z = 1 << 2,
}