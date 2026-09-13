using System;
using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Entities;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Managers;

public abstract partial class ResourceIndicatorManager<T> : Node3D, IResIndicatorManager, IStandardResourceHandler
    where T : IResIndicatorColor
{
    [Export] protected PackedScene _indicator = null!;
    private IEntity _entity = null!;

    public void SetEntity(IEntity entity)
    {
        _entity = entity;
        SetResourceFrom(entity);
    }

    protected abstract void SetResourceFrom(IEntity entity);

    public void OnConsumed(int hp)
    {
        var indicator = _indicator.Instantiate<IResIndicator<T>>();
        CreateNewIndicator(indicator, indicator.OnConsumed, hp);
    }

    public void OnGenerated(int hp)
    {
        var indicator = _indicator.Instantiate<IResIndicator<T>>();
        CreateNewIndicator(indicator, indicator.OnGenerated, hp);
    }

    protected void CreateNewIndicator(IResIndicator<T> indicator, Action action)
    {
        indicator.SetClientRelation(_entity.GetClientRelation());
        action();
        TryAddChild(indicator);
    }

    protected void CreateNewIndicator(IResIndicator<T> indicator, Action<int> action, int hp)
    {
        indicator.SetClientRelation(_entity.GetClientRelation());
        TryAddChild(indicator);
        action(hp);
    }

    private void TryAddChild(IResIndicator<T> indicator)
    {
        if (indicator is Node node)
            AddChild(node);
        
        indicator.SetWorldPosition(GlobalPosition);
    }
}