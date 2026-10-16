using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Resources.Indicators.Standard;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Managers;

[GlobalClass]
public partial class FocusIndicatorManager : ResourceIndicatorManager<StdResIndicatorColor>, IFocusHandler
{
    private IStandardResource? _focus = null!;

    protected override void SetResourceFrom(IEntity entity)
    {
        if (!entity.ResourceManager.TryGetResource(ResourceRegistry.GetId<IFocus>(), out IStandardResource? res))
            return;

        if (_focus == res)
            return;

        if (_focus != null)
            this.Unbind(_focus);

        _focus = res;

        if (_focus != null)
            this.Bind(_focus);
    }
}