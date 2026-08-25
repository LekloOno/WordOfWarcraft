using System.Collections.Generic;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Debug;

[GlobalClass]
public partial class TestPlayerAttack : Node
{
    [Export(PropertyHint.Layers2DPhysics)] private uint _targetTeamMask;
    [Export] private float _range   = 3f;
    [Export] private int _damage    = 10;

    private IEntity _entity = null!;

    public override void _Ready()
    {
        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.J)
				Attack();
		}
    }

    private void Attack()
    {
        ICollection<IEntity> targets = _entity.GetTargetsInRange(_range, _targetTeamMask);
        GD.Print($"Players attacks {targets.Count} targets..");
        foreach (IEntity target in targets)
            target.Health.Damage(_damage);
    }
}