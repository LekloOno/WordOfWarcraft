using System;
using Godot;

namespace WowGd.Src.Render.Animation.Shakes;

public interface IShakeOutput
{
    event Action<Vector3> Shaken;
}