using Godot;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Health;

namespace WowGd.Src.Render.WorldRenderer.Entities;

public partial class SimpleDude : EntityRender3D
{
    [Export] private Sprite3D _bodySprite = null!;
    [Export] private HealthBar _healthBar = null!;
    [Export] private StandardResourceBar _focusBar = null!;
    [Export] private DamageIndicatorManager _damageIndicatorManager = null!;

    public override bool InitSpec(IEntity entity)
    {
        _healthBar.SetHealth(entity.Health);
        _damageIndicatorManager.SetHealth(entity.Health, entity);

        if (entity.ResourceManager.Focus is Focus focus)
            _focusBar.SetResource(focus);
        else
            _focusBar.Hide();
            
        return true;
    }

}
