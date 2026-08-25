using System.Collections.Generic;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Health;
using WowGd.Src.Tools;

namespace WowGd.Src.Debug;

[GlobalClass]
public partial class TestPlayerAttack : Node, IEntityHealthHandler
{
    [Export(PropertyHint.Layers2DPhysics)] private uint _targetTeamMask;
    [Export] private float _range   = 3f;
    [Export] private int   _hp      = 10;

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
            if (keyEvent.Keycode == Key.K)
                Heal();
            if (keyEvent.Keycode == Key.L)
                Resurrect();
		}
    }

    private void Resurrect()
    {
        ICollection<IEntity> targets = _entity.GetTargetsInRange(_range, _targetTeamMask);
        GD.Print($"Players resurrects {targets.Count} targets..");
        foreach (IEntity target in targets)
            target.Health.Resurrect();
    }

    private void Heal()
    {
        ICollection<IEntity> targets = _entity.GetTargetsInRange(_range, _targetTeamMask);
        GD.Print($"Players heals {targets.Count} targets..");
        foreach (IEntity target in targets)
            target.Health.Heal(_hp);
    }

    private void Attack()
    {
        ICollection<IEntity> targets = _entity.GetTargetsInRange(_range, _targetTeamMask);
        GD.Print($"Players attacks {targets.Count} targets..");
        foreach (IEntity target in targets)
            target.Health.Damage(_hp);
    }

    public void OnDied()
    {
        GD.Print("alo");
        SetProcessUnhandledKeyInput(false);
    }

    public void OnRessurected(int hp)
    {
        SetProcessUnhandledKeyInput(true);
    }

    public void OnDamaged(int hp) {}
    public void OnHealed(int hp) {}
}