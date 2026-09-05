using Godot;
using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui;

namespace WowGd.Src.Render.WorldRenderer.Entities;

public partial class ComplexPlayer : SimpleDude
{
    [Export]
    private WorderDisplay _worder = null!;

    public override bool InitSpec(IEntity entity)
    {
        if (entity.ActuatorDriver is not PlayerDactyloDriver driver)
            return false;

        _worder.Driver = driver;
        return base.InitSpec(entity);
    }
}
