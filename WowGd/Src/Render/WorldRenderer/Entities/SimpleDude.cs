using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Health;

namespace WowGd.Src.Render.WorldRenderer.Entities;

public partial class SimpleDude : EntityRender3D
{
    [Export] private Sprite3D _bodySprite = null!;
    [Export] private HealthBar _healthBar = null!;
    [Export] private DamageIndicatorManager _damageIndicatorManager = null!;

    public override void Init(IEntity entity)
    {
        Entity = entity;
        _healthBar.SetHealth(entity.Health);
        _damageIndicatorManager.SetHealth(entity.Health);
    }
}
