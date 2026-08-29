using Godot;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Combat.Health.Handlers;

[GlobalClass]
public partial class MoveModeHealthHandler : HealthHandler<MoveMode>
{
    [Export] public MoveMode MoveMode
    {
        get => _component;
        set => _component = value;
    }

    public override void OnDamaged(int hp) {}
    public override void OnHealed(int hp) {}

    public override void OnResurrected(int hp) =>
        MoveMode.Enable();

    public override void OnDied() =>
        MoveMode.Disable();
}