using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Managers;

[GlobalClass]
public partial class ResourceIndicatorSuperManager : Node3D
{
    public void BindEntity(IEntity entity)
    {
        foreach (Node node in GetChildren())
            if (node is IResIndicatorManager manager)
                manager.SetEntity(entity);
    }
}