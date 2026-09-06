using Godot;
using WowGd.Src.Combat.Health.Handlers;

namespace WowGd.Src.Combat.Modes;

[GlobalClass]
public partial class CombatModeHealthHandler : HealthHandler<CombatMode>
{
    public override void OnResurrected(int hp) =>
        _component.Enable();

    public override void OnDied() =>
        _component.Disable();

    public override void OnDamaged(int hp) {}
    public override void OnHealed(int hp) {}
}