using System;
using Godot;
using WowGd.Src.Combat.Health;

namespace WowGd.Src.Render.Ui.Combat.Health;

[GlobalClass]
public partial class DamageIndicatorManager : Node3D, IEntityHealthHandler
{
    [Export] private PackedScene _indicator = null!;
    private IEntityHealth _health = null!;

    public void SetHealth(IEntityHealth health)
    {
        if (_health == health)
            return;

        if (_health != null)
            this.Unbind(_health);

        _health = health;
        this.Bind(_health);
    }

    public void OnDamaged(int hp)
    {
        var indicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(indicator, indicator.OnDamaged, hp);
    }

    public void OnHealed(int hp)
    {
        var indicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(indicator, indicator.OnHealed, hp);
    }

    public void OnResurrected(int hp)
    {
        var indicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(indicator, indicator.OnResurrected, hp);

        var healIndicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(healIndicator, healIndicator.OnHealed, hp);
    }

    public void OnDied()
    {
        var indicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(indicator, indicator.OnDied);
    }

    private void CreateNewIndicator(IDamageIndicator indicator, Action action)
    {
        action();
        TryAddChild(indicator);
    }

    private void CreateNewIndicator(IDamageIndicator indicator, Action<int> action, int hp)
    {
        TryAddChild(indicator);
        action(hp);
    }

    private void TryAddChild(IDamageIndicator indicator)
    {
        if (indicator is Node node)
            AddChild(node);
        
        indicator.SetWorldPosition(GlobalPosition);
    }
}