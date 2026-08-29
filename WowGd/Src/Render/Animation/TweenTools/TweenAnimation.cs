using Godot;
using Godot.Collections;

namespace WowGd.Src.Render.Animation.TweenTools;

[GlobalClass]
public partial class TweenAnimation : Resource
{
    [Export] public string EventName = "";
    [Export] public float SpeedMultiplier = 1.0f;
    [Export] public Array<TweenSettings> Steps = new();
    [Export] public bool Parallel = false;
}
