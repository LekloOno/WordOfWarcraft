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

    public void OnConsumed(int hp)
    {
		GD.Print($"Hit sent !\t {_health.Current} (-{hp})");
    }

    public void OnDied()
    {
        GD.Print($"Final blow !");
    }

    public void OnGenerated(int hp)
    {
        GD.Print($"Healed !\t {_health.Current} (+{hp})");
    }

    public void OnResurrected(int hp)
    {
        GD.Print($"Resurrected.\t hp: {_health.Current}");
    }
}