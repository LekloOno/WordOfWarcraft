using Godot;
using WowGd.Src.Combat.Health;

namespace WowGd.Src.Debug;

[GlobalClass]
public partial class SelfHarmDbg : Node
{
	[Export] private EntityHealth _health = null!;
	[Export] private int _damageHp = 5;
	[Export] private int _healHp = 5;

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.O)
				Damage();
			else if (keyEvent.Keycode == Key.P)
				Heal();
			else if (keyEvent.Keycode == Key.R)
				Resurrect();
		}
	}

    private void Resurrect()
    {
        _health.Resurrect();
		GD.Print($"Resurrected with {_health.Current} hp.\t hp: {_health.Current}");
    }

    private void Heal()
    {
        if (_health.Heal(_healHp))
			GD.Print("Can't heal a dead target. Press R to ressurect.");
		else
			GD.Print($"healed with {_healHp} hp.\t hp: {_health.Current}");
    }

    private void Damage()
    {
        bool killed = _health.Damage(_damageHp);
		string killedStr = killed ? "killing blow !" : "still alive";
		GD.Print($"hitting with {_damageHp} hp : {killedStr}\t hp: {_health.Current}");
    }
}