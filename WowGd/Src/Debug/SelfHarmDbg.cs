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
				_health.Damage(_damageHp);
			else if (keyEvent.Keycode == Key.P)
				_health.Heal(_healHp);
			else if (keyEvent.Keycode == Key.R)
				_health.Resurrect();
		}
	}
}