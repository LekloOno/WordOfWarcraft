using Godot;

namespace WowGd.Src.Debug;

public partial class GameLineEdit : TextEdit
{
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.CtrlPressed)
			{
				switch (keyEvent.Keycode)
				{
					case Key.A:
					case Key.C:
					case Key.V:
					case Key.X:
					case Key.Z:
						AcceptEvent();
						return;
				}
			}
		}

		base._GuiInput(@event);
	}
}
