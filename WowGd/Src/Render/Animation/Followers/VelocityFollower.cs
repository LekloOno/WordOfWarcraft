using Godot;
using WowGd.Src.Render.Animation.Smoothing;

namespace WowGd.Src.Render.Animation.Followers;

[GlobalClass]
public partial class VelocityFollower : Node2DFollower
{
    private PositionVelDamperData   _velDamperData = null!;
    private PositionVelDamper       _velDamper = null!;

    [Export] public PositionVelDamperData VelDamperData
    {
        get => _velDamperData;
        set
        {
            if (value == _velDamperData)
                return;

            _velDamperData = value;

            if (_velDamper != null)
                _velDamper.Data = value;
        }    
    }

    protected override void ReadySpec()
    {
        _velDamper = new(_velDamperData);
    }

    public override void _PhysicsProcess(double delta)
    {
        _velDamper.Process(Node.GlobalPosition, (float)delta);
        GlobalPosition = _velDamper.Position;
    }
}