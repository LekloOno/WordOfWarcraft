using Godot;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Combat.Health.Handlers;

[GlobalClass]
public partial class MoverHealthHandler : HealthHandler<BodyMover>
{
    [Export] public BodyMover Mover
    {
        get => _component;
        set => _component = value;
    }

    public override void OnConsumed(int hp) {}
    public override void OnGenerated(int hp) {}

    public override void OnResurrected(int hp) =>
        Mover.Enable();

    public override void OnDied() =>
        Mover.Disable();
}