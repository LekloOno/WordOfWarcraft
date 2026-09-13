using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Resources.Indicators.Health;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Managers;

[GlobalClass]
public partial class DamageIndicatorManager : ResourceIndicatorManager<DamageIndicatorColor>, IEntityHealthHandler
{
    private IEntityHealth _health = null!;

    protected override void SetResourceFrom(IEntity entity)
    {
        if (_health == entity.Health)
            return;

        if (_health != null)
            this.Unbind(_health);

        _health = entity.Health;
        this.Bind(_health);
    }

    public void OnResurrected(int hp)
    {
        var indicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(indicator, indicator.OnResurrected, hp);

        var healIndicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(healIndicator, healIndicator.OnGenerated, hp);
    }

    public void OnDied()
    {
        var indicator = _indicator.Instantiate<IDamageIndicator>();
        CreateNewIndicator(indicator, indicator.OnDied);
    }
}