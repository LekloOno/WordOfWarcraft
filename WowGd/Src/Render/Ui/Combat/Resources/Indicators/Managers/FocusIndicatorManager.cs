using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Resources.Indicators.Standard;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Managers;

[GlobalClass]
public partial class FocusIndicatorManager : ResourceIndicatorManager<StdResIndicatorColor>, IFocusHandler
{
    private IFocus? _focus = null!;

    protected override void SetResourceFrom(IEntity entity)
    {
        if (_focus == entity.ResourceManager.Focus)
            return;

        if (_focus != null)
            this.Unbind(_focus);

        _focus = entity.ResourceManager.Focus;

        if (_focus != null)
            this.Bind(_focus);
    }
}