using Godot;
using WowGd.Src.Debug;

namespace WowGd.Src.Combat.Health.Handlers;

[GlobalClass]
public partial class RandomModeHealthHandler : HealthHandler<RandomMode>
{
    [Export] public RandomMode Mode
    {
        get => _component;
        set => _component = value;
    }

    public override void OnDamaged(int hp) {}
    public override void OnHealed(int hp) {}

    public override void OnRessurected(int hp) =>
        _component.Enable();

    public override void OnDied() =>
        _component.Disable();
}