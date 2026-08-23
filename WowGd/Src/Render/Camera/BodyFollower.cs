using Godot;
using WowGd.Src.Physics;
using WowGd.Src.Tools;

namespace WowGd.Src.Render.Camera;

[GlobalClass]
public partial class BodyFollower : Node2D
{
    [Export] private Body _body = null!;
    [Export] private float _lerpSpeed = 1f;

    public override void _Process(double delta)
    {
        Position = Position.ProcessLerp(_body.Position, _lerpSpeed, (float)delta);
    }
}
