using Godot;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Resources.Bars;
using WowGd.Src.Render.Ui.Combat.Resources.Indicators.Managers;
using WowGd.Src.Render.Ui.Movement;

namespace WowGd.Src.Render.WorldRenderer.Entities;

public partial class SimpleDude : EntityRender3D
{
    [Export] private Sprite3D _bodySprite = null!;
    [Export] private HealthBar _healthBar = null!;
    [Export] private StandardResourceBar _focusBar = null!;
    [Export] private ResourceIndicatorSuperManager _indicatorSuperManager = null!;
    [Export] private TackleUi _tackleUi = null!;

    public override bool InitSpec(IEntity entity)
    {
        _healthBar.SetHealth(entity.Health);
        _indicatorSuperManager.BindEntity(entity);

        if (entity.ResourceManager.Focus is Focus focus)
            _focusBar.SetResource(focus);
        else
            _focusBar.Hide();

        _tackleUi.Bind(entity.EntityMover.DynamicTackleNode);
            
        return true;
    }

}
