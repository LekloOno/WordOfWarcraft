using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Tools;

namespace WowGd.Src.Debug;

[GlobalClass]
public partial class EntityHealthLogger : Node, IEntityHealthHandler
{
    private IEntityHealth _health = null!;

    public override void _Ready()
    {
        if (this.TryGetComposed(out _health!))
            this.Bind(_health);
    }

    public void OnDamaged(int hp)
    {
		GD.Print($"Hit sent !\t {_health.CurrentHps} (-{hp})");
    }

    public void OnDied()
    {
        GD.Print($"Final blow !");
    }

    public void OnHealed(int hp)
    {
        GD.Print($"Healed !\t {_health.CurrentHps} (+{hp})");
    }

    public void OnResurrected(int hp)
    {
        GD.Print($"Resurrected.\t hp: {_health.CurrentHps}");
    }
}