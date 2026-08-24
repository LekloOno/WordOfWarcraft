using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Render.Animation.Followers;

[GlobalClass]
public partial class LerpedFollower : Node2DFollower
{
    [Export] private float _lerpSpeed = 1f;

    public override void _Process(double delta)
    {
        Position = Position.ProcessLerp(_node.Position, _lerpSpeed, (float)delta);
    }
}
